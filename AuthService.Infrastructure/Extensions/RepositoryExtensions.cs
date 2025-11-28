using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.DbContexts;
using AuthService.Infrastructure.Persistance.Repositories;
using AuthService.Infrastructure.Persistance.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositoryRegistrations(this IServiceCollection services)
        {
            services.AddScoped<IEntityMapper, EntityMapper>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,,>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<IUnitOfWork>(provider =>
            {
                var dbContext = provider.GetRequiredService<AuthDbContext>();

                return new UnitOfWork(
                    dbContext,
                    ctx => provider.GetRequiredService<IUserRepository>(),
                    ctx => provider.GetRequiredService<ITenantRepository>(),
                    ctx => provider.GetRequiredService<IRefreshTokenRepository>()
                );
            });

            return services;
        }
    }
}
