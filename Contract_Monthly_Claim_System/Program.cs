using Contract_Monthly_Claim_System.Data;
using Contract_Monthly_Claim_System.Models;
using Contract_Monthly_Claim_System.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Linq;

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework and Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Register custom services
builder.Services.AddScoped<IExportService, ExportService>();

var app = builder.Build();

// Ensure database schema is present and then seed roles/users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        // Log the effective connection string so you can verify which DB is targeted
        try
        {
            var conn = db.Database.GetDbConnection();
            logger.LogInformation("Effective DB ConnectionString: {cs}", conn.ConnectionString);

            // Check whether AspNetUsers exists in that database
            try
            {
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers'";
                var result = await cmd.ExecuteScalarAsync();
                var count = Convert.ToInt32(result);
                logger.LogInformation("AspNetUsers table exists: {exists}", count > 0);
                await conn.CloseAsync();
            }
            catch (Exception exQuery)
            {
                logger.LogWarning(exQuery, "Failed to query INFORMATION_SCHEMA.TABLES for AspNetUsers.");
            }
        }
        catch (Exception exConn)
        {
            logger.LogWarning(exConn, "Failed to read DB connection information.");
        }

        // Determine whether EF Core migrations exist in the assembly.
        var migrationsInAssembly = db.Database.GetMigrations(); // migrations available in project
        var migrationCount = migrationsInAssembly?.Count() ?? 0;
        logger.LogInformation("Migrations available in assembly: {count}", migrationCount);

        if (migrationCount > 0)
        {
            try
            {
                // Apply pending migrations
                db.Database.Migrate();
                logger.LogInformation("Database migrated successfully.");
            }
            catch (Exception migrateEx)
            {
                logger.LogWarning(migrateEx, "Migrate() failed - attempting EnsureCreated() as fallback.");
                try
                {
                    db.Database.EnsureCreated();
                    logger.LogInformation("EnsureCreated() succeeded as fallback after Migrate() failure.");
                }
                catch (Exception ensureEx)
                {
                    logger.LogError(ensureEx, "EnsureCreated() also failed.");
                    throw; // rethrow so startup logs show the failure
                }
            }
        }
        else
        {
            // No migrations present in project: create schema directly from current model
            logger.LogWarning("No EF Core migrations found in assembly; calling EnsureCreated() to create schema.");
            try
            {
                db.Database.EnsureCreated();
                logger.LogInformation("Database schema created via EnsureCreated().");
            }
            catch (Exception ensureEx)
            {
                logger.LogError(ensureEx, "EnsureCreated() failed when no migrations were present.");
                throw;
            }
        }

        await DbInitializer.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger2 = services.GetRequiredService<ILogger<Program>>();
        logger2.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
