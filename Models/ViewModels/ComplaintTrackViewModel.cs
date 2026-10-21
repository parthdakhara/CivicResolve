using System.ComponentModel.DataAnnotations;

namespace CivicResolve.Models.ViewModels
{
    public class ComplaintTrackViewModel
    {
        [Required(ErrorMessage = "Please enter a Tracking ID (e.g., CR-2026-10492) or 10-digit Phone Number.")]
        [Display(Name = "Tracking Number or Registered Mobile")]
        public string SearchQuery { get; set; } = string.Empty;

        public Complaint? Complaint { get; set; }

        public List<Complaint> RelatedComplaints { get; set; } = new List<Complaint>();

        public bool HasSearched { get; set; } = false;

        public FeedbackCreateViewModel? FeedbackForm { get; set; }
    }
}
