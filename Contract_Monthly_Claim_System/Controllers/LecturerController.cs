using Contract_Monthly_Claim_System.Data;
using Contract_Monthly_Claim_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Contract_Monthly_Claim_System.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public LecturerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var claims = await _context.Claims
                .Include(c => c.Documents)
                .Where(c => c.LecturerId == user!.Id)
                .OrderByDescending(c => c.SubmittedDate)
                .ToListAsync();

            ViewBag.UserName = $"{user!.FirstName} {user.LastName}";
            ViewBag.TotalClaims = claims.Count;
            ViewBag.PendingClaims = claims.Count(c => c.Status == "Pending");
            ViewBag.ApprovedClaims = claims.Count(c => c.Status == "Approved");

            return View(claims);
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(SubmitClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var claim = new Claim
                {
                    LecturerId = user!.Id,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    TotalAmount = model.HoursWorked * model.HourlyRate,
                    Notes = model.Notes,
                    SubmittedDate = DateTime.Now,
                    Status = "Pending"
                };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                // Handle file uploads
                if (model.Documents != null && model.Documents.Any())
                {
                    foreach (var file in model.Documents)
                    {
                        if (file.Length > 0)
                        {
                            using var memoryStream = new MemoryStream();
                            await file.CopyToAsync(memoryStream);

                            var document = new ClaimDocument
                            {
                                ClaimId = claim.ClaimId,
                                FileName = file.FileName,
                                FileType = file.ContentType,
                                FileData = memoryStream.ToArray(),
                                UploadedDate = DateTime.Now
                            };

                            _context.ClaimDocuments.Add(document);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction(nameof(Dashboard));
            }

            return View(model);
        }

        public async Task<IActionResult> ClaimDetails(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
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
