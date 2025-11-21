using Microsoft.AspNetCore.Identity;

namespace Contract_Monthly_Claim_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation property - use your domain Claim type (no System.Security.Claims using)
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}
