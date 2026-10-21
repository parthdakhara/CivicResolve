using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CivicResolve.Models
{
    public class CitizenFeedback
    {
        public int Id { get; set; }

        public int ComplaintId { get; set; }

        [ForeignKey("ComplaintId")]
        public Complaint? Complaint { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Feedback comment cannot exceed 500 characters.")]
        public string? Comments { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
