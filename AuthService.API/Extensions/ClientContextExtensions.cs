using AuthService.API.Contexts;
using AuthService.Application.Interfaces.Contexts;

namespace AuthService.API.Extensions
{
    public static class ClientContextExtensions
    {
        public static IServiceCollection AddClientContext(this IServiceCollection services)
        {
            services.AddScoped<IClientContext, ClientContext>();
            return services;
        }
    }
}
