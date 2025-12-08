using AuthService.Application.Common;

namespace AuthService.API.Extensions
{
    public static class TokenOptionsExtensions
    {
        public static IServiceCollection AddTokenOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenOptions>(configuration.GetSection("TokenOptions"));
            return services;
        }
    }
}
