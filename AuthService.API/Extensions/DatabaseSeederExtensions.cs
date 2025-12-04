using AuthService.Infrastructure.Persistance.DbContexts;
using AuthService.Infrastructure.Persistance.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

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

            db.Tenants.Add(new Tenant
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
            });

            await db.SaveChangesAsync();
        }
    }
}
