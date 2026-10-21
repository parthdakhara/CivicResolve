using CivicResolve.Data;
using CivicResolve.Models;
using CivicResolve.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CivicResolve.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string SessionKeyRole = "AdminRole";

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            EnsureAdminSession();

            var total = await _context.Complaints.CountAsync();
            var pending = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Submitted || c.Status == ComplaintStatus.UnderReview);
            var inProgress = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.InProgress || c.Status == ComplaintStatus.Assigned);
            var resolved = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Resolved);
            var rejected = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Rejected);
            var critical = await _context.Complaints.CountAsync(c => c.Priority == ComplaintPriority.Critical && c.Status != ComplaintStatus.Resolved && c.Status != ComplaintStatus.Rejected);

            var now = DateTime.UtcNow;
            var overdue = await _context.Complaints
                .CountAsync(c => c.Status != ComplaintStatus.Resolved && 
                                 c.Status != ComplaintStatus.Rejected && 
                                 c.SlaDeadline.HasValue && 
                                 c.SlaDeadline.Value < now);

            var feedbacks = await _context.CitizenFeedbacks.ToListAsync();
            double avgRating = feedbacks.Any() ? Math.Round(feedbacks.Average(f => f.Rating), 1) : 4.8;
            double resRate = total > 0 ? Math.Round((double)resolved / total * 100, 1) : 0;

            // Department stats
            var departments = await _context.Departments
                .Include(d => d.Complaints)
                .Where(d => d.IsActive)
                .ToListAsync();

            var deptStats = departments.Select(d => new DepartmentStatItem
            {
                DepartmentName = d.Name,
                DepartmentCode = d.Code,
                Total = d.Complaints.Count,
                Resolved = d.Complaints.Count(c => c.Status == ComplaintStatus.Resolved),
                Pending = d.Complaints.Count(c => c.Status != ComplaintStatus.Resolved && c.Status != ComplaintStatus.Rejected)
            }).ToList();

            // Recent complaints
            var recentComplaints = await _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.Category)
                .OrderByDescending(c => c.CreatedAt)
                .Take(6)
                .ToListAsync();

            // Overdue complaints
            var overdueList = await _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.Category)
                .Where(c => c.Status != ComplaintStatus.Resolved && 
                             c.Status != ComplaintStatus.Rejected && 
                             c.SlaDeadline.HasValue && 
                             c.SlaDeadline.Value < now)
                .OrderBy(c => c.SlaDeadline)
                .Take(5)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalComplaints = total,
                PendingComplaints = pending,
                InProgressComplaints = inProgress,
                ResolvedComplaints = resolved,
                RejectedComplaints = rejected,
                OverdueComplaints = overdue,
                CriticalComplaints = critical,
                ResolutionRate = resRate,
                AverageSatisfactionRating = avgRating,
                DepartmentStats = deptStats,
                RecentComplaints = recentComplaints,
                OverdueList = overdueList
            };

            return View(viewModel);
        }

        // GET: /Admin/Index
        public async Task<IActionResult> Index(ComplaintFilterViewModel filter)
        {
            EnsureAdminSession();

            var query = _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.Category)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(c => c.TrackingNumber.ToLower().Contains(term) ||
                                         c.Title.ToLower().Contains(term) ||
                                         c.CitizenName.ToLower().Contains(term) ||
                                         c.CitizenPhone.Contains(term) ||
                                         c.Address.ToLower().Contains(term));
            }

            if (filter.DepartmentId.HasValue && filter.DepartmentId > 0)
            {
                query = query.Where(c => c.DepartmentId == filter.DepartmentId.Value);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(c => c.Status == filter.Status.Value);
            }

            if (filter.Priority.HasValue)
            {
                query = query.Where(c => c.Priority == filter.Priority.Value);
            }

            if (filter.WardNumber.HasValue && filter.WardNumber > 0)
            {
                query = query.Where(c => c.WardNumber == filter.WardNumber.Value);
            }

            if (filter.OnlyOverdue == true)
            {
                var now = DateTime.UtcNow;
                query = query.Where(c => c.Status != ComplaintStatus.Resolved && 
                                         c.Status != ComplaintStatus.Rejected && 
                                         c.SlaDeadline.HasValue && 
                                         c.SlaDeadline.Value < now);
            }

            filter.TotalItems = await query.CountAsync();

            if (filter.CurrentPage < 1) filter.CurrentPage = 1;

            filter.Complaints = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((filter.CurrentPage - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            filter.Departments = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(filter);
        }

        // GET: /Admin/Details/5
        public async Task<IActionResult> Details(int id)
        {
            EnsureAdminSession();

            var complaint = await _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.Category)
                .Include(c => c.StatusHistories.OrderByDescending(h => h.ChangedAt))
                .Include(c => c.Feedbacks.OrderByDescending(f => f.SubmittedAt))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
            {
                return NotFound();
            }

            return View(complaint);
        }

        // GET: /Admin/EditStatus/5
        public async Task<IActionResult> EditStatus(int id)
        {
            EnsureAdminSession();

            var complaint = await _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.Category)
                .Include(c => c.StatusHistories.OrderByDescending(h => h.ChangedAt))
                .Include(c => c.Feedbacks)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
            {
                return NotFound();
            }

            var viewModel = new ComplaintEditViewModel
            {
                Id = complaint.Id,
                TrackingNumber = complaint.TrackingNumber,
                Title = complaint.Title,
                CitizenName = complaint.CitizenName,
                CitizenPhone = complaint.CitizenPhone,
                CitizenEmail = complaint.CitizenEmail,
                Address = complaint.Address,
                WardNumber = complaint.WardNumber,
                CategoryName = complaint.Category?.Name ?? "General",
                DepartmentName = complaint.Department?.Name ?? "General",
                Description = complaint.Description,
                AttachmentPath = complaint.AttachmentPath,
                CreatedAt = complaint.CreatedAt,
                SlaDeadline = complaint.SlaDeadline,
                Status = complaint.Status,
                Priority = complaint.Priority,
                DepartmentId = complaint.DepartmentId,
                AssignedOfficer = complaint.AssignedOfficer,
                OfficerRemarks = complaint.OfficerRemarks ?? string.Empty,
                StatusHistories = complaint.StatusHistories.ToList(),
                Feedbacks = complaint.Feedbacks.ToList()
            };

            await PopulateDepartmentsDropDownAsync(complaint.DepartmentId);
            return View(viewModel);
        }

        // POST: /Admin/EditStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(ComplaintEditViewModel model)
        {
            EnsureAdminSession();

            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsDropDownAsync(model.DepartmentId);
                return View(model);
            }

            var complaint = await _context.Complaints
                .Include(c => c.StatusHistories)
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (complaint == null)
            {
                return NotFound();
            }

            var oldStatus = complaint.Status;
            var currentOfficer = HttpContext.Session.GetString(SessionKeyRole) ?? "Ward Officer";

            // Update complaint fields
            complaint.Status = model.Status;
            complaint.Priority = model.Priority;
            complaint.DepartmentId = model.DepartmentId;
            complaint.AssignedOfficer = model.AssignedOfficer?.Trim();
            complaint.OfficerRemarks = model.OfficerRemarks.Trim();
            complaint.UpdatedAt = DateTime.UtcNow;

            if (model.Status == ComplaintStatus.Resolved && complaint.ResolvedAt == null)
            {
                complaint.ResolvedAt = DateTime.UtcNow;
            }
            else if (model.Status != ComplaintStatus.Resolved)
            {
                complaint.ResolvedAt = null;
            }

            // Append Audit Trail History
            var history = new ComplaintStatusHistory
            {
                ComplaintId = complaint.Id,
                OldStatus = oldStatus,
                NewStatus = model.Status,
                ActionBy = string.IsNullOrWhiteSpace(model.AssignedOfficer) ? currentOfficer : model.AssignedOfficer.Trim(),
                Remarks = model.OfficerRemarks.Trim(),
                ChangedAt = DateTime.UtcNow
            };

            _context.ComplaintStatusHistories.Add(history);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Complaint {complaint.TrackingNumber} status updated to '{model.Status}' successfully.";
            return RedirectToAction(nameof(Details), new { id = complaint.Id });
        }

        // POST: /Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            EnsureAdminSession();

            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
            {
                return NotFound();
            }

            var tracking = complaint.TrackingNumber;
            _context.Complaints.Remove(complaint);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Complaint record {tracking} was successfully deleted.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/SwitchRole
        [HttpPost]
        public IActionResult SwitchRole(string role)
        {
            if (!string.IsNullOrWhiteSpace(role))
            {
                HttpContext.Session.SetString(SessionKeyRole, role);
                TempData["InfoMessage"] = $"Switched active officer role to: {role}";
            }
            return RedirectToAction(nameof(Dashboard));
        }

        // GET: /Admin/ExportCsv
        public async Task<IActionResult> ExportCsv()
        {
            var complaints = await _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.Category)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("TrackingID,Title,Department,Category,Priority,Status,Ward,CitizenName,Phone,Email,CreatedDate,ResolvedDate,AssignedOfficer");

            foreach (var c in complaints)
            {
                csv.AppendLine($"\"{c.TrackingNumber}\",\"{EscapeCsv(c.Title)}\",\"{c.Department?.Name}\",\"{c.Category?.Name}\",\"{c.Priority}\",\"{c.Status}\",\"{c.WardNumber}\",\"{EscapeCsv(c.CitizenName)}\",\"{c.CitizenPhone}\",\"{c.CitizenEmail}\",\"{c.CreatedAt:yyyy-MM-dd HH:mm}\",\"{c.ResolvedAt:yyyy-MM-dd HH:mm}\",\"{EscapeCsv(c.AssignedOfficer ?? string.Empty)}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"CivicResolve_Report_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv");
        }

        #region Helpers
        private void EnsureAdminSession()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyRole)))
            {
                HttpContext.Session.SetString(SessionKeyRole, "Senior Municipal Officer");
            }
            ViewBag.CurrentRole = HttpContext.Session.GetString(SessionKeyRole);
        }

        private async Task PopulateDepartmentsDropDownAsync(int? selectedId = null)
        {
            var departments = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewBag.DepartmentList = new SelectList(departments, "Id", "Name", selectedId);
        }

        private string EscapeCsv(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace("\"", "\"\"");
        }
        #endregion
    }
}
