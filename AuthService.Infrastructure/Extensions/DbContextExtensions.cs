using AuthService.Infrastructure.Persistance.Configuration;
using AuthService.Infrastructure.Persistance.DbContexts;
using AuthService.Infrastructure.Persistance.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AuthService.Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static IServiceCollection ConfigureDbConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseConfigurations>(configuration.GetSection("DatabaseConfigurations"));
            return services;
        }
        public static IServiceCollection AddAuthDbContext(this IServiceCollection services)
        {
            services.AddDbContext<AuthDbContext>((serviceProvider, opt) =>
            {
                var dbConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfigurations>>().Value;
                opt.UseNpgsql(dbConfig.ConnectionString);
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
            {
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.AllowedForNewUsers = true;

                opt.User.RequireUniqueEmail = true;
                
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequiredLength = 5;

            })
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
