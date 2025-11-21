using Contract_Monthly_Claim_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

using DomainClaim = Contract_Monthly_Claim_System.Models.Claim;

namespace Contract_Monthly_Claim_System.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger("DbInitializer") ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

            try
            {
                // Ensure database is created/migrated
                await context.Database.MigrateAsync();

                // Create roles
                string[] roleNames = { "Lecturer", "Coordinator", "AcademicManager", "HR" };
                foreach (var roleName in roleNames)
                {
                    try
                    {
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                            if (!roleResult.Succeeded)
                            {
                                logger.LogWarning("Failed to create role {Role}: {Errors}", roleName, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                            }
                        }
                    }
                    catch (Exception exRole)
                    {
                        logger.LogError(exRole, "Error creating role {Role}", roleName);
                    }
                }

                // Create default users
                await CreateDefaultUser(userManager, logger, "lecturer@cmcs.com", "Lecturer123!", "John", "Doe", "Lecturer");
                await CreateDefaultUser(userManager, logger, "coordinator@cmcs.com", "Coordinator123!", "Jane", "Smith", "Coordinator");
                await CreateDefaultUser(userManager, logger, "manager@cmcs.com", "Manager123!", "Bob", "Johnson", "AcademicManager");
                await CreateDefaultUser(userManager, logger, "hr@cmcs.com", "HR123!", "Alice", "Williams", "HR");

                // Create sample claims
                await CreateSampleClaims(context, userManager, logger);
            }
            catch (Exception ex)
            {
                // Catch-all so app startup doesn't crash silently — logged for diagnosis
                logger.LogError(ex, "Unhandled exception while initializing the database.");
                // Do not rethrow here so application can still start (adjust if you prefer fail-fast).
            }
        }

        private static async Task CreateDefaultUser(UserManager<ApplicationUser> userManager, ILogger logger, string email, string password, string firstName, string lastName, string role)
        {
            try
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        Role = role,
                        EmailConfirmed = true,
                        CreatedDate = DateTime.Now
                    };

                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        var addRoleResult = await userManager.AddToRoleAsync(user, role);
                        if (!addRoleResult.Succeeded)
                        {
                            logger.LogWarning("Failed to add user {Email} to role {Role}: {Errors}", email, role, string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger.LogWarning("Failed to create user {Email}: {Errors}", email, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating default user {Email}", email);
            }
        }

        private static async Task CreateSampleClaims(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger logger)
        {
            try
            {
                if (!context.Claims.Any())
                {
                    var lecturer = await userManager.FindByEmailAsync("lecturer@cmcs.com");

                    if (lecturer != null)
                    {
                        var claims = new List<DomainClaim>
                        {
                            new DomainClaim
                            {
                                LecturerId = lecturer.Id,
                                HoursWorked = 40,
                                HourlyRate = 250.00m,
                                TotalAmount = 10000.00m,
                                Notes = "October 2024 - Programming Fundamentals",
                                SubmittedDate = DateTime.Now.AddDays(-5),
                                Status = "Pending"
                            },
                            new DomainClaim
                            {
                                LecturerId = lecturer.Id,
                                HoursWorked = 35,
                                HourlyRate = 250.00m,
                                TotalAmount = 8750.00m,
                                Notes = "September 2024 - Database Systems",
                                SubmittedDate = DateTime.Now.AddDays(-30),
                                Status = "Approved",
                                ApprovedDate = DateTime.Now.AddDays(-25)
                            },
                            new DomainClaim
                            {
                                LecturerId = lecturer.Id,
                                HoursWorked = 20,
                                HourlyRate = 250.00m,
                                TotalAmount = 5000.00m,
                                Notes = "August 2024 - Web Development",
                                SubmittedDate = DateTime.Now.AddDays(-60),
                                Status = "Rejected",
                                RejectionReason = "Insufficient documentation provided"
                            }
                        };

                        context.Claims.AddRange(claims);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        logger.LogWarning("Lecturer account not found; skipping sample claims creation.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating sample claims.");
            }
        }
    }
}
