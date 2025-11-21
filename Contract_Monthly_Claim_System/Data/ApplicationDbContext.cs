using Contract_Monthly_Claim_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Contract_Monthly_Claim_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Claim> Claims { get; set; }
        public DbSet<ClaimDocument> ClaimDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Claim entity
            builder.Entity<Claim>()
                .HasKey(c => c.ClaimId);

            builder.Entity<Claim>()
                .HasOne(c => c.Lecturer)
                .WithMany(u => u.Claims)
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Claim>()
                .Property(c => c.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<Claim>()
                .Property(c => c.HourlyRate)
                .HasPrecision(18, 2);

            // Configure ClaimDocument entity
            builder.Entity<ClaimDocument>()
                .HasKey(d => d.DocumentId);

            builder.Entity<ClaimDocument>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
