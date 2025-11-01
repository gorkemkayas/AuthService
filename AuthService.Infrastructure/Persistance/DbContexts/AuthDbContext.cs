
using AuthService.Infrastructure.Persistance.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistance.DbContexts
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
            base.OnModelCreating(builder);
        }

    }
}
