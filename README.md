# PROG6212_PART-3
PART 3_ASSIGNMENTS
## Project Structure

\`\`\`
ContractMonthlyClaimSystem/
├── Controllers/
│   ├── AccountController.cs
│   ├── LecturerController.cs
│   ├── CoordinatorController.cs
│   ├── AcademicManagerController.cs
│   ├── HRController.cs
│   └── HomeController.cs
├── Models/
│   ├── ApplicationUser.cs
│   ├── Claim.cs
│   └── Document.cs
├── ViewModels/
│   ├── RegisterViewModel.cs
│   ├── LoginViewModel.cs
│   ├── ClaimViewModel.cs
│   ├── ApprovalViewModel.cs
│   └── DocumentViewModel.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   └── Error.cshtml
│   ├── Account/
│   │   ├── Register.cshtml
│   │   └── Login.cshtml
│   ├── Lecturer/
│   │   ├── Dashboard.cshtml
│   │   ├── SubmitClaim.cshtml
│   │   ├── MyClaims.cshtml
│   │   └── ClaimDetails.cshtml
│   ├── Coordinator/
│   │   ├── Dashboard.cshtml
│   │   ├── ReviewClaim.cshtml
│   │   ├── PendingClaims.cshtml
│   │   └── AllClaims.cshtml
│   ├── AcademicManager/
│   │   ├── Dashboard.cshtml
│   │   ├── ReviewClaim.cshtml
│   │   ├── AllClaims.cshtml
│   │   └── Reports.cshtml
│   ├── HR/
│   │   ├── Dashboard.cshtml
│   │   ├── ApprovedClaims.cshtml
│   │   ├── ClaimDetails.cshtml
│   │   ├── AllClaims.cshtml
│   │   ├── LecturerManagement.cshtml
│   │   └── Reports.cshtml
│   └── Home/
│       └── Index.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   ├── js/
│   │   └── site.js
│   └── uploads/
├── Program.cs
├── appsettings.json
└── ContractMonthlyClaimSystem.csproj
\`\`\`
