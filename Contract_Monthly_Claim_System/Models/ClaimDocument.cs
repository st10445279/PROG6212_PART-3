using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contract_Monthly_Claim_System.Models
{
    public class ClaimDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public int ClaimId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FileType { get; set; } = string.Empty;

        [Required]
        public byte[] FileData { get; set; } = Array.Empty<byte>();

        public DateTime UploadedDate { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("ClaimId")]
        public Claim? Claim { get; set; }
    }
}
