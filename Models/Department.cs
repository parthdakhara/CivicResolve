using System.ComponentModel.DataAnnotations;

namespace CivicResolve.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Department name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department code is required.")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Code must be between 2 and 10 characters.")]
        public string Code { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Head of Department (HOD) name is required.")]
        [StringLength(100, ErrorMessage = "HOD name cannot exceed 100 characters.")]
        public string HodName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Official email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string ContactEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact phone number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be a valid 10-digit number.")]
        public string ContactPhone { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
