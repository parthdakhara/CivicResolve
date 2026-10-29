using CivicResolve.Data;
using CivicResolve.Models;
using CivicResolve.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CivicResolve.Controllers
{
    public class ComplaintController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private const string SessionKeyRecentTracking = "RecentTrackingNumbers";

        public ComplaintController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: /Complaint/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDepartmentsDropDownAsync();
            return View(new ComplaintCreateViewModel());
        }

        // POST: /Complaint/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplaintCreateViewModel model)
        {
            // Validate Foreign Keys exist in database
            if (!model.DepartmentId.HasValue || !await _context.Departments.AnyAsync(d => d.Id == model.DepartmentId.Value && d.IsActive))
            {
                ModelState.AddModelError("DepartmentId", "Please select a valid municipal department.");
            }

            var category = model.CategoryId.HasValue ? await _context.Categories.FindAsync(model.CategoryId.Value) : null;
            if (category == null || (model.DepartmentId.HasValue && category.DepartmentId != model.DepartmentId.Value))
            {
                ModelState.AddModelError("CategoryId", "Please select a valid grievance category for the chosen department.");
            }

            // Validate File Upload if present
            if (model.Attachment != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                var extension = Path.GetExtension(model.Attachment.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Attachment", "Only image files (.jpg, .jpeg, .png) or PDF documents are permitted.");
                }

                if (model.Attachment.Length > 5 * 1024 * 1024) // 5 MB
                {
                    ModelState.AddModelError("Attachment", "File size cannot exceed 5 MB.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsDropDownAsync(model.DepartmentId);
                if (model.DepartmentId.HasValue)
                {
                    var catList = await _context.Categories
                        .Where(c => c.DepartmentId == model.DepartmentId.Value)
                        .OrderBy(c => c.Name)
                        .ToListAsync();
                    ViewBag.CategoryList = new SelectList(catList, "Id", "Name", model.CategoryId);
                }
                return View(model);
            }

            // Generate Unique Tracking ID
            string trackingNumber;
            do
            {
                var randomNum = new Random().Next(10000, 99999);
                trackingNumber = $"CR-2026-{randomNum}";
            } while (await _context.Complaints.AnyAsync(c => c.TrackingNumber == trackingNumber));

            // Determine SLA Deadline from Category
            int slaHours = category?.SlaHours ?? 48;
            var slaDeadline = DateTime.UtcNow.AddHours(slaHours);

            // Handle File Attachment Upload
            string? attachmentRelativePath = null;
            if (model.Attachment != null && model.Attachment.Length > 0)
            {
                var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolder = Path.Combine(webRoot, "uploads", "complaints");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{trackingNumber}_{Guid.NewGuid():N}{Path.GetExtension(model.Attachment.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Attachment.CopyToAsync(fileStream);
                }

                attachmentRelativePath = $"/uploads/complaints/{uniqueFileName}";
            }

            var complaint = new Complaint
            {
                TrackingNumber = trackingNumber,
                CitizenName = model.CitizenName.Trim(),
                CitizenEmail = model.CitizenEmail.Trim(),
                CitizenPhone = model.CitizenPhone.Trim(),
                WardNumber = model.WardNumber,
                Address = model.Address.Trim(),
                Landmark = model.Landmark?.Trim(),
                DepartmentId = model.DepartmentId!.Value,
                CategoryId = model.CategoryId!.Value,
                Priority = model.Priority,
                Status = ComplaintStatus.Submitted,
                Title = model.Title.Trim(),
                Description = model.Description.Trim(),
                AttachmentPath = attachmentRelativePath,
                CreatedAt = DateTime.UtcNow,
                SlaDeadline = slaDeadline
            };

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            // Create initial audit log
            var history = new ComplaintStatusHistory
            {
                ComplaintId = complaint.Id,
                OldStatus = ComplaintStatus.Submitted,
                NewStatus = ComplaintStatus.Submitted,
                ActionBy = "Citizen (Online Portal)",
                Remarks = "Grievance successfully submitted and registered in the municipal portal.",
                ChangedAt = DateTime.UtcNow
            };

            _context.ComplaintStatusHistories.Add(history);
            await _context.SaveChangesAsync();

            // State Management: Store tracking number in Citizen Session
            SaveTrackingNumberToSession(trackingNumber);

            // State Management: TempData Flash Alerts
            TempData["SuccessMessage"] = $"Grievance lodged successfully! Your Tracking ID is {trackingNumber}.";
            TempData["NewTrackingNumber"] = trackingNumber;

            return RedirectToAction(nameof(Track), new { q = trackingNumber });
        }

        // GET & POST: /Complaint/Track?q=CR-2026-10101
        public async Task<IActionResult> Track(string? q)
        {
            var viewModel = new ComplaintTrackViewModel
            {
                SearchQuery = q ?? string.Empty
            };

            if (string.IsNullOrWhiteSpace(q))
            {
                return View(viewModel);
            }

            viewModel.HasSearched = true;
            var query = q.Trim();

            // Try find by exact Tracking Number
            var complaint = await _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.Category)
                .Include(c => c.StatusHistories.OrderByDescending(h => h.ChangedAt))
                .Include(c => c.Feedbacks.OrderByDescending(f => f.SubmittedAt))
                .FirstOrDefaultAsync(c => c.TrackingNumber.ToUpper() == query.ToUpper());

            if (complaint != null)
            {
                viewModel.Complaint = complaint;
                viewModel.FeedbackForm = new FeedbackCreateViewModel
                {
                    ComplaintId = complaint.Id,
                    TrackingNumber = complaint.TrackingNumber
                };

                // Remember this tracking in session if not already there
                SaveTrackingNumberToSession(complaint.TrackingNumber);
            }
            else
            {
                // Search by Phone number
                var complaintsByPhone = await _context.Complaints
                    .Include(c => c.Department)
                    .Include(c => c.Category)
                    .Where(c => c.CitizenPhone == query)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                if (complaintsByPhone.Any())
                {
                    viewModel.RelatedComplaints = complaintsByPhone;
                    if (complaintsByPhone.Count == 1)
                    {
                        viewModel.Complaint = complaintsByPhone.First();
                        viewModel.FeedbackForm = new FeedbackCreateViewModel
                        {
                            ComplaintId = viewModel.Complaint.Id,
                            TrackingNumber = viewModel.Complaint.TrackingNumber
                        };
                    }
                }
            }

            return View(viewModel);
        }

        // GET: /Complaint/MyComplaints (Demonstrates Session State Management)
        [HttpGet]
        public async Task<IActionResult> MyComplaints()
        {
            var trackingNumbers = GetTrackingNumbersFromSession();
            var complaints = new List<Complaint>();

            if (trackingNumbers.Any())
            {
                complaints = await _context.Complaints
                    .Include(c => c.Department)
                    .Include(c => c.Category)
                    .Where(c => trackingNumbers.Contains(c.TrackingNumber))
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }

            ViewBag.SessionActive = true;
            return View(complaints);
        }

        // POST: /Complaint/Feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Feedback(FeedbackCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Failed to submit feedback. Please provide a valid rating (1-5) and comment.";
                return RedirectToAction(nameof(Track), new { q = model.TrackingNumber });
            }

            var complaint = await _context.Complaints.FindAsync(model.ComplaintId);
            if (complaint == null)
            {
                return NotFound();
            }

            var feedback = new CitizenFeedback
            {
                ComplaintId = model.ComplaintId,
                Rating = model.Rating,
                Comments = model.Comments.Trim(),
                SubmittedAt = DateTime.UtcNow
            };

            _context.CitizenFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thank you! Your feedback has been recorded and shared with municipal supervisors.";
            return RedirectToAction(nameof(Track), new { q = model.TrackingNumber });
        }

        // AJAX API: /Complaint/GetCategoriesByDepartment?departmentId=1
        [HttpGet]
        public async Task<JsonResult> GetCategoriesByDepartment(int departmentId)
        {
            var categories = await _context.Categories
                .Where(c => c.DepartmentId == departmentId)
                .OrderBy(c => c.Name)
                .Select(c => new { id = c.Id, name = c.Name, slaHours = c.SlaHours, description = c.Description })
                .ToListAsync();

            return Json(categories);
        }

        #region Helper Methods for State Management
        private void SaveTrackingNumberToSession(string trackingNumber)
        {
            var numbers = GetTrackingNumbersFromSession();
            if (!numbers.Contains(trackingNumber))
            {
                numbers.Insert(0, trackingNumber);
                var json = JsonSerializer.Serialize(numbers);
                HttpContext.Session.SetString(SessionKeyRecentTracking, json);
            }
        }

        private List<string> GetTrackingNumbersFromSession()
        {
            var json = HttpContext.Session.GetString(SessionKeyRecentTracking);
            if (string.IsNullOrEmpty(json))
            {
                return new List<string>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private async Task PopulateDepartmentsDropDownAsync(int? selectedDepartmentId = null)
        {
            var departments = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewBag.DepartmentList = new SelectList(departments, "Id", "Name", selectedDepartmentId);
        }
        #endregion
    }
}
