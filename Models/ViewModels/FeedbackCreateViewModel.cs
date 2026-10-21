using System.ComponentModel.DataAnnotations;

namespace CivicResolve.Models.ViewModels
{
    public class FeedbackCreateViewModel
    {
        [Required]
        public int ComplaintId { get; set; }

        public string TrackingNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a satisfaction rating between 1 and 5 stars.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        [Display(Name = "Satisfaction Rating")]
        public int Rating { get; set; } = 5;

        [Required(ErrorMessage = "Please share brief remarks regarding the resolution quality.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Comments must be between 5 and 500 characters.")]
        [Display(Name = "Citizen Feedback Comments")]
        public string Comments { get; set; } = string.Empty;
    }
}
