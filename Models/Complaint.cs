using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CivicResolve.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string TrackingNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Citizen full name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string CitizenName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string CitizenEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be a valid 10-digit number.")]
        public string CitizenPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ward number is required.")]
        [Range(1, 150, ErrorMessage = "Ward number must be between 1 and 150.")]
        public int WardNumber { get; set; }

        [Required(ErrorMessage = "Incident address is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters.")]
        public string Address { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Landmark cannot exceed 100 characters.")]
        public string? Landmark { get; set; }

        [Required(ErrorMessage = "Please select a grievance category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        [Required]
        public ComplaintPriority Priority { get; set; } = ComplaintPriority.Medium;

        [Required]
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Submitted;

        [Required(ErrorMessage = "Complaint title/subject is required.")]
        [StringLength(120, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 120 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Detailed description of the issue is required.")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Description must be between 20 and 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(255)]
        public string? AttachmentPath { get; set; }

        [StringLength(100)]
        public string? AssignedOfficer { get; set; }

        [StringLength(1000)]
        public string? OfficerRemarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? SlaDeadline { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public bool IsOverdue => Status != ComplaintStatus.Resolved && 
                                 Status != ComplaintStatus.Rejected && 
                                 SlaDeadline.HasValue && 
                                 DateTime.UtcNow > SlaDeadline.Value;

        // Navigation
        public ICollection<ComplaintStatusHistory> StatusHistories { get; set; } = new List<ComplaintStatusHistory>();
        public ICollection<CitizenFeedback> Feedbacks { get; set; } = new List<CitizenFeedback>();
    }
}
