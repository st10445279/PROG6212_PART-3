namespace Contract_Monthly_Claim_System.Models
{
    public class ApprovalViewModel
    {
        public int ClaimId { get; set; }
        public string LecturerName { get; set; } = string.Empty;
        public string LecturerEmail { get; set; } = string.Empty;
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ClaimDocument> Documents { get; set; } = new List<ClaimDocument>();
        public string? RejectionReason { get; set; }
    }
}
