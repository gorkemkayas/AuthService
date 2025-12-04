using AuthService.Application.Interfaces.Services;
using AuthService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServiceRegistrations(this IServiceCollection services)
        {
            // Application service registrations
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITenantService, TenantService>();
            return services;
        }
    }
}
