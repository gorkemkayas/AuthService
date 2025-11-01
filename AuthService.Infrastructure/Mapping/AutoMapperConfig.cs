using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Mapping
{
    public static class AutoMapperConfig
    {
        public static void AddAutoMapperConfigurations(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperConfig).Assembly);
        }
    }
}
