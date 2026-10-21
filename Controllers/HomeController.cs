using System.Diagnostics;
using CivicResolve.Data;
using CivicResolve.Models;
using CivicResolve.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CivicResolve.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var total = await _context.Complaints.CountAsync();
            var resolved = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Resolved);
            var inProgress = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.InProgress || c.Status == ComplaintStatus.Assigned);
            var pending = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Submitted || c.Status == ComplaintStatus.UnderReview);
            var deptCount = await _context.Departments.CountAsync(d => d.IsActive);

            var departments = await _context.Departments
                .Include(d => d.Categories)
                .Where(d => d.IsActive)
                .Take(6)
                .ToListAsync();

            var recentResolutions = await _context.Complaints
                .Include(c => c.Department)
                .Include(c => c.Category)
                .Where(c => c.Status == ComplaintStatus.Resolved)
                .OrderByDescending(c => c.ResolvedAt ?? c.UpdatedAt ?? c.CreatedAt)
                .Take(4)
                .ToListAsync();

            ViewBag.Total = total;
            ViewBag.Resolved = resolved;
            ViewBag.InProgress = inProgress;
            ViewBag.Pending = pending;
            ViewBag.DeptCount = deptCount;
            ViewBag.Departments = departments;
            ViewBag.RecentResolutions = recentResolutions;

            return View();
        }

        public async Task<IActionResult> Faq()
        {
            var departments = await _context.Departments
                .Include(d => d.Categories)
                .Where(d => d.IsActive)
                .ToListAsync();
            return View(departments);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
