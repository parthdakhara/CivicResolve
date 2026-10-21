using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CivicResolve.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Associated department is required.")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        [Range(1, 720, ErrorMessage = "SLA response time must be between 1 and 720 hours (30 days).")]
        [Display(Name = "SLA Resolution Window (Hours)")]
        public int SlaHours { get; set; } = 48;

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string? Description { get; set; }

        // Navigation
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
