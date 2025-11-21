# PROG6212_PART-3
PART 3_ASSIGNMENTS

# Contract Monthly Claim System (CMCS)

A comprehensive ASP.NET Core MVC web application (.NET 9.0) that streamlines the submission, verification, approval, and payment of monthly claims for independent/contract lecturers.

## Features Overview

- Multi-level approval workflow  
- Role-based access control (Lecturer → Coordinator → Academic Manager → HR)  
- Secure file uploads with validation  
- Real-time claim status tracking  
- Auto-calculation of claim totals  
- Detailed dashboards and reports per role  

## Roles & Permissions

| Role              | Key Functions                                                                 |
|-------------------|-------------------------------------------------------------------------------|
| **Lecturer**      | Submit claims, upload supporting documents, view personal claim history       |
| **Coordinator**   | Review & approve/reject submitted claims, add remarks                         |
| **Academic Manager** | Final approval/rejection of coordinator-approved claims, view reports      |
| **HR Personnel**  | View fully approved claims, mark as paid, manage lecturers, generate payroll reports |

## Technology Stack

- ASP.NET Core 9.0 MVC  
- Entity Framework Core (Code-First)  
- SQL Server (LocalDB or full instance)  
- ASP.NET Core Identity (Individual Accounts)  
- Bootstrap 5 + custom CSS + vanilla JavaScript  

## Prerequisites

- Visual Studio 2022/2026 or VS Code  
- .NET 9.0 SDK  
- SQL Server / LocalDB  
- Node.js (optional – only for additional frontend tooling)

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


### 1. Create Project
1. Open Visual Studio  
2. New Project → ASP.NET Core Web App (Model-View-Controller)  
3. Framework: .NET 9.0  
4. Authentication: **Individual User Accounts**

   

### 2. Install NuGet Packages
```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
