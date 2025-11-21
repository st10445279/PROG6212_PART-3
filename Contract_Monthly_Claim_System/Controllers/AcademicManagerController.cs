using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Contract_Monthly_Claim_System.Data;
using Contract_Monthly_Claim_System.Models;

namespace Contract_Monthly_Claim_System.Controllers
{
    [Authorize(Roles = "AcademicManager")]
    public class AcademicManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AcademicManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var pendingClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.SubmittedDate)
                .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            ViewBag.UserName = $"{user!.FirstName} {user.LastName}";
            ViewBag.TotalPending = pendingClaims.Count;
            ViewBag.TotalPendingAmount = pendingClaims.Sum(c => c.TotalAmount);

            return View(pendingClaims);
        }

        public async Task<IActionResult> AllClaims()
        {
            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .OrderByDescending(c => c.SubmittedDate)
                .ToListAsync();

            return View(claims);
        }

        public async Task<IActionResult> ReviewClaim(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            if (claim == null)
            {
                return NotFound();
            }

            var viewModel = new ApprovalViewModel
            {
                ClaimId = claim.ClaimId,
                LecturerName = $"{claim.Lecturer!.FirstName} {claim.Lecturer.LastName}",
                LecturerEmail = claim.Lecturer.Email ?? "",
                HoursWorked = claim.HoursWorked,
                HourlyRate = claim.HourlyRate,
                TotalAmount = claim.TotalAmount,
                Notes = claim.Notes,
                SubmittedDate = claim.SubmittedDate,
                Status = claim.Status,
                Documents = claim.Documents.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = "Approved";
            claim.ApprovedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim approved successfully!";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectClaim(int id, string rejectionReason)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = "Rejected";
            claim.RejectionReason = rejectionReason;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim rejected successfully!";
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> DownloadDocument(int id)
        {
            var document = await _context.ClaimDocuments.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            return File(document.FileData, document.FileType, document.FileName);
        }
    }
}
