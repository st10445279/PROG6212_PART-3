using System.ComponentModel.DataAnnotations;

namespace Contract_Monthly_Claim_System.Models
{
    public class SubmitClaimViewModel
    {
        [Required(ErrorMessage = "Hours worked is required")]
        [Range(1, 500, ErrorMessage = "Hours worked must be between 1 and 500")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(100, 10000, ErrorMessage = "Hourly rate must be between R100 and R10,000")]
        [Display(Name = "Hourly Rate (ZAR)")]
        public decimal HourlyRate { get; set; }

        [StringLength(500)]
        [Display(Name = "Additional Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Supporting Documents")]
        public List<IFormFile>? Documents { get; set; }
    }
}
