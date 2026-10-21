using System.ComponentModel.DataAnnotations;

namespace CivicResolve.Models.ViewModels
{
    public class ComplaintEditViewModel
    {
        public int Id { get; set; }

        public string TrackingNumber { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string CitizenName { get; set; } = string.Empty;

        public string CitizenPhone { get; set; } = string.Empty;

        public string CitizenEmail { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int WardNumber { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? AttachmentPath { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? SlaDeadline { get; set; }

        // Editable fields
        [Required(ErrorMessage = "Please select a status.")]
        [Display(Name = "Current Workflow Status")]
        public ComplaintStatus Status { get; set; }

        [Required(ErrorMessage = "Please select priority level.")]
        [Display(Name = "Priority Level")]
        public ComplaintPriority Priority { get; set; }

        [Required(ErrorMessage = "Assigned department is required.")]
        [Display(Name = "Assigned Department")]
        public int DepartmentId { get; set; }

        [StringLength(100, ErrorMessage = "Officer name cannot exceed 100 characters.")]
        [Display(Name = "Assigned Field Inspector / Officer")]
        public string? AssignedOfficer { get; set; }

        [Required(ErrorMessage = "Official remarks/progress notes are required when updating status.")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Remarks must be between 5 and 1000 characters.")]
        [Display(Name = "Official Action / Resolution Remarks")]
        public string OfficerRemarks { get; set; } = string.Empty;

        public List<ComplaintStatusHistory> StatusHistories { get; set; } = new List<ComplaintStatusHistory>();
        public List<CitizenFeedback> Feedbacks { get; set; } = new List<CitizenFeedback>();
    }
}
