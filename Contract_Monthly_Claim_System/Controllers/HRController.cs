using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Contract_Monthly_Claim_System.Data;
using Contract_Monthly_Claim_System.Models;
using Contract_Monthly_Claim_System.Services;

namespace Contract_Monthly_Claim_System.Controllers
{
    [Authorize(Roles = "HR")]
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IExportService _exportService;

        public HRController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IExportService exportService)
        {
            _context = context;
            _userManager = userManager;
            _exportService = exportService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var approvedClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == "Approved")
                .OrderByDescending(c => c.ApprovedDate)
                .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            ViewBag.UserName = $"{user!.FirstName} {user.LastName}";
            ViewBag.TotalApproved = approvedClaims.Count;
            ViewBag.TotalPayable = approvedClaims.Sum(c => c.TotalAmount);

            return View(approvedClaims);
        }

        public async Task<IActionResult> ApprovedClaims()
        {
            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .Where(c => c.Status == "Approved")
                .OrderByDescending(c => c.ApprovedDate)
                .ToListAsync();

            return View(claims);
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

        public async Task<IActionResult> ClaimDetails(int id)
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
                Documents = claim.Documents.ToList(),
                RejectionReason = claim.RejectionReason
            };

            return View(viewModel);
        }

        public async Task<IActionResult> LecturerManagement()
        {
            var users = await _userManager.Users.ToListAsync();
            var lecturers = users.Where(u => u.Role == "Lecturer").ToList();

            return View(lecturers);
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var approvedClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == "Approved")
                .ToListAsync();

            var fileBytes = _exportService.ExportClaimsToExcel(approvedClaims);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ApprovedClaims_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        public async Task<IActionResult> ExportToPdf()
        {
            var approvedClaims = await _context.Claims
                .Include(c => c.Lecturer)
                .Where(c => c.Status == "Approved")
                .ToListAsync();

            var fileBytes = _exportService.ExportClaimsToPdf(approvedClaims);
            return File(fileBytes, "application/pdf", $"ApprovedClaims_{DateTime.Now:yyyyMMdd}.pdf");
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
