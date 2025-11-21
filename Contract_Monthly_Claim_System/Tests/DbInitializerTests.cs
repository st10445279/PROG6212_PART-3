using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contract_Monthly_Claim_System.Data;
using Contract_Monthly_Claim_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Contract_Monthly_Claim_System.Tests
{
    public class DbInitializerTests
    {
        private ApplicationDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        private UserManager<ApplicationUser> CreateUserManagerMock(ConcurrentDictionary<string, ApplicationUser> store)
        {
            var storeMock = new Mock<IUserStore<ApplicationUser>>();
            var um = new UserManager<ApplicationUser>(storeMock.Object,
                Options.Create(new IdentityOptions()), new PasswordHasher<ApplicationUser>(),
                Array.Empty<IUserValidator<ApplicationUser>>(),
                Array.Empty<IPasswordValidator<ApplicationUser>>(),
                new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(),
                new ServiceCollection().BuildServiceProvider(), NullLogger<UserManager<ApplicationUser>>.Instance);

            // Replace methods we need via Moq on the concrete user manager via subclassing alternative:
            // For test simplicity, wrap calls into a lightweight custom implementation:
            var mock = new Mock<UserManager<ApplicationUser>>(storeMock.Object,
                Options.Create(new IdentityOptions()), new PasswordHasher<ApplicationUser>(),
                Array.Empty<IUserValidator<ApplicationUser>>(), Array.Empty<IPasswordValidator<ApplicationUser>>(),
                new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(),
                new ServiceCollection().BuildServiceProvider(), NullLogger<UserManager<ApplicationUser>>.Instance);

            mock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) =>
                {
                    store.TryGetValue(email, out var u);
                    return u;
                });

            mock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser u, string pwd) =>
                {
                    u.Id ??= Guid.NewGuid().ToString();
                    // Email may be nullable in type; use null-forgiving here because tests populate emails
                    store[u.Email!] = u;
                    return IdentityResult.Success;
                });

            mock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            return mock.Object;
        }

        private RoleManager<IdentityRole> CreateRoleManagerMock(ConcurrentDictionary<string, IdentityRole> roles)
        {
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            var mock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object,
                Array.Empty<IRoleValidator<IdentityRole>>(), new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), NullLogger<RoleManager<IdentityRole>>.Instance);

            mock.Setup(m => m.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync((string r) => roles.ContainsKey(r));

            mock.Setup(m => m.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync((IdentityRole r) =>
                {
                    // Role.Name may be nullable on IdentityRole; use null-forgiving because tests set names
                    roles[r.Name!] = r;
                    return IdentityResult.Success;
                });

            return mock.Object; 
        }

        [Fact]
        public async Task Initialize_CreatesRolesUsersAndClaims()
        {
            var context = CreateInMemoryContext(nameof(Initialize_CreatesRolesUsersAndClaims));
            var userStore = new ConcurrentDictionary<string, ApplicationUser>();
            var roleStore = new ConcurrentDictionary<string, IdentityRole>();

            var userManager = CreateUserManagerMock(userStore);
            var roleManager = CreateRoleManagerMock(roleStore);

            var services = new ServiceCollection();
            services.AddSingleton(userManager);
            services.AddSingleton(roleManager);
            services.AddSingleton<ApplicationDbContext>(context);
            services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
            var provider = services.BuildServiceProvider();

            await DbInitializer.Initialize(provider);

            // After initialization there should be roles created
            Assert.True(roleStore.ContainsKey("Lecturer"));
            Assert.True(roleStore.ContainsKey("Coordinator"));
            Assert.True(roleStore.ContainsKey("AcademicManager"));
            Assert.True(roleStore.ContainsKey("HR"));

            // Users created
            Assert.True(userStore.ContainsKey("lecturer@cmcs.com"));
            Assert.True(userStore.ContainsKey("coordinator@cmcs.com"));
            Assert.True(userStore.ContainsKey("manager@cmcs.com"));
            Assert.True(userStore.ContainsKey("hr@cmcs.com"));

            // Claims created (3 sample claims)
            var claimsCount = context.Claims.Count();
            Assert.Equal(3, claimsCount);
        }

        [Fact]
        public async Task Initialize_LogsAndContinuesOnRoleCreationFailure()
        {
            var context = CreateInMemoryContext(nameof(Initialize_LogsAndContinuesOnRoleCreationFailure));
            var userStore = new ConcurrentDictionary<string, ApplicationUser>();
            var roleStore = new ConcurrentDictionary<string, IdentityRole>();

            // Create a RoleManager that throws on CreateAsync to simulate failure
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            var failingRoleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object,
                Array.Empty<IRoleValidator<IdentityRole>>(), new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), NullLogger<RoleManager<IdentityRole>>.Instance);
            failingRoleManagerMock.Setup(m => m.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            failingRoleManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityRole>())).ThrowsAsync(new Exception("role-creation-failed"));

            var userManager = CreateUserManagerMock(userStore);
            var roleManager = failingRoleManagerMock.Object;

            var services = new ServiceCollection();
            services.AddSingleton(userManager);
            services.AddSingleton(roleManager);
            services.AddSingleton<ApplicationDbContext>(context);
            services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance);
            var provider = services.BuildServiceProvider();

            // Should not throw even though role creation throws internally
            await DbInitializer.Initialize(provider);

            // Users should still be created and sample claims created
            Assert.True(userStore.ContainsKey("lecturer@cmcs.com"));
            Assert.Equal(3, context.Claims.Count());
        }
    }
}