using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CivicResolve.Models.ViewModels
{
    public class ComplaintCreateViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 100 characters.")]
        [Display(Name = "Citizen Full Name")]
        public string CitizenName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Valid email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email format (e.g., citizen@example.com).")]
        [Display(Name = "Email Address")]
        public string CitizenEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "10-digit mobile number is required.")]
        [RegularExpression(@"^[6-9][0-9]{9}$", ErrorMessage = "Please enter a valid 10-digit Indian/standard mobile number.")]
        [Display(Name = "Mobile Number")]
        public string CitizenPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Municipal Ward Number is required.")]
        [Range(1, 150, ErrorMessage = "Ward number must be between 1 and 150.")]
        [Display(Name = "Ward / Zone No.")]
        public int WardNumber { get; set; }

        [Required(ErrorMessage = "Exact address or location is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters.")]
        [Display(Name = "Street Address / Location")]
        public string Address { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Landmark cannot exceed 100 characters.")]
        [Display(Name = "Nearby Landmark (Optional)")]
        public string? Landmark { get; set; }

        [Required(ErrorMessage = "Please select the concerned department.")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Please select the grievance category.")]
        [Display(Name = "Grievance Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please specify the urgency/priority level.")]
        [Display(Name = "Severity / Priority")]
        public ComplaintPriority Priority { get; set; } = ComplaintPriority.Medium;

        [Required(ErrorMessage = "Brief subject/title of the issue is required.")]
        [StringLength(120, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 120 characters.")]
        [Display(Name = "Issue Title / Subject")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Detailed description is required.")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Please describe the problem with at least 20 characters.")]
        [Display(Name = "Detailed Grievance Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Photo Proof / Document (Optional)")]
        public IFormFile? Attachment { get; set; }
    }
}
