using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CivicResolve.Models
{
    public class ComplaintStatusHistory
    {
        public int Id { get; set; }

        public int ComplaintId { get; set; }

        [ForeignKey("ComplaintId")]
        public Complaint? Complaint { get; set; }

        public ComplaintStatus OldStatus { get; set; }

        public ComplaintStatus NewStatus { get; set; }

        [StringLength(100)]
        public string ActionBy { get; set; } = "System";

        [StringLength(500)]
        public string Remarks { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
