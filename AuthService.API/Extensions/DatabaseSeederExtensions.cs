using AuthService.Infrastructure.Persistance.DbContexts;
using AuthService.Infrastructure.Persistance.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthService.API.Extensions
{
    public static class DatabaseSeederExtensions
    {
        public static async Task SeedDefaultTenantAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            // Migration'ları uygula (opsiyonel ama önerilir)
            await db.Database.MigrateAsync();

            // Eğer kayıt varsa hiç dokunma
            if (await db.Tenants.AnyAsync())
                return;

            var tenant = new Tenant
            {
                Id = 1,
                CreatedAt = new DateTime(2002, 02, 14, 0, 0, 0, DateTimeKind.Utc), // postgresql requirement
                Domain = "https://default.kayas.dev",
                Email = "default@kayas.dev",
                IsActive = true,
                IsSystem = true,
                Name = "Default Tenant",
                HashedPassword = "0000000000",
                IsDeleted = false
            };

            db.Tenants.Add(tenant);
            await db.SaveChangesAsync();

            // Create Identity role and admin user using RoleManager / UserManager
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            const string superAdminRoleName = "SuperAdmin";
            if (!await roleManager.RoleExistsAsync(superAdminRoleName))
            {
                var role = new ApplicationRole
                {
                    Name = superAdminRoleName,
                    Description = "Global project administrator",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                await roleManager.CreateAsync(role);
            }
            var tenantAdminRoleName = "TenantAdmin";
            if (!await roleManager.RoleExistsAsync(tenantAdminRoleName))
            {
                var role = new ApplicationRole
                {
                    Name = tenantAdminRoleName,
                    Description = "Tenant administrator",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                await roleManager.CreateAsync(role);
            }

            // Admin password: prefer to supply via configuration in non-dev environments
            var adminPassword = config["Seed:AdminPassword"] ?? "ChangeMe!12345"; // override in production

            // Create admin user (email == tenant.Email)
            var adminEmail = tenant.Email;
            var existing = await userManager.FindByEmailAsync(adminEmail);
            if (existing == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Name = "System",
                    Surname = "Admin",
                    TenantId = tenant.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    // Log failures for debugging; do not throw in seeder in production
                    Console.WriteLine("Failed to create admin user: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    return;
                }

                var addRoleResult = await userManager.AddToRoleAsync(adminUser, superAdminRoleName);
                if (!addRoleResult.Succeeded)
                {
                    Console.WriteLine("Failed to assign role to admin user: " + string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
