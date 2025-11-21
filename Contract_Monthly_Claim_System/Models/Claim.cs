using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contract_Monthly_Claim_System.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        [Required]
        public string LecturerId { get; set; } = string.Empty;

        [Required]
        [Range(1, 500, ErrorMessage = "Hours worked must be between 1 and 500")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Range(100, 10000, ErrorMessage = "Hourly rate must be between R100 and R10,000")]
        public decimal HourlyRate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime? ApprovedDate { get; set; }

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        // Navigation properties
        [ForeignKey("LecturerId")]
        public ApplicationUser? Lecturer { get; set; }

        public ICollection<ClaimDocument> Documents { get; set; } = new List<ClaimDocument>();
    }
}
