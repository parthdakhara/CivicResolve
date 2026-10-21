using CivicResolve.Data;
using CivicResolve.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CivicResolve.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Department
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments
                .Include(d => d.Categories)
                .Include(d => d.Complaints)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(departments);
        }

        // GET: /Department/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Department());
        }

        // POST: /Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (await _context.Departments.AnyAsync(d => d.Code.ToUpper() == department.Code.ToUpper()))
            {
                ModelState.AddModelError("Code", "A department with this code already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(department);
            }

            department.Code = department.Code.ToUpper().Trim();
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Department '{department.Name}' was successfully registered.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Department/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Categories)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: /Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (await _context.Departments.AnyAsync(d => d.Code.ToUpper() == department.Code.ToUpper() && d.Id != id))
            {
                ModelState.AddModelError("Code", "Another department already uses this code.");
            }

            if (!ModelState.IsValid)
            {
                return View(department);
            }

            try
            {
                var existing = await _context.Departments.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Name = department.Name.Trim();
                existing.Code = department.Code.ToUpper().Trim();
                existing.Description = department.Description?.Trim();
                existing.HodName = department.HodName.Trim();
                existing.ContactEmail = department.ContactEmail.Trim();
                existing.ContactPhone = department.ContactPhone.Trim();
                existing.IsActive = department.IsActive;

                _context.Update(existing);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Department '{existing.Name}' details updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Departments.AnyAsync(d => d.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        // POST: /Department/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Complaints)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            if (department.Complaints.Any())
            {
                TempData["ErrorMessage"] = $"Cannot delete '{department.Name}' because it has {department.Complaints.Count} associated complaint records. You may deactivate it instead.";
                return RedirectToAction(nameof(Index));
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Department '{department.Name}' deleted.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Department/AddCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(int departmentId, string name, int slaHours, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["ErrorMessage"] = "Category name cannot be empty.";
                return RedirectToAction(nameof(Edit), new { id = departmentId });
            }

            var category = new Category
            {
                DepartmentId = departmentId,
                Name = name.Trim(),
                SlaHours = slaHours <= 0 ? 48 : slaHours,
                Description = description?.Trim()
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Grievance category '{category.Name}' added.";
            return RedirectToAction(nameof(Edit), new { id = departmentId });
        }

        // POST: /Department/DeleteCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int categoryId, int departmentId)
        {
            var category = await _context.Categories
                .Include(c => c.Complaints)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Complaints.Any())
            {
                TempData["ErrorMessage"] = $"Cannot remove category '{category.Name}' because it contains active complaint records.";
                return RedirectToAction(nameof(Edit), new { id = departmentId });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Category '{category.Name}' removed.";
            return RedirectToAction(nameof(Edit), new { id = departmentId });
        }
    }
}
