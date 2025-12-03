using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.DbContexts;
using AuthService.Infrastructure.Persistance.Entities;
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

            // Generic repository registrations (lambda ile closed type)
            services.AddScoped<IGenericRepository<User, string>>(provider =>
                new GenericRepository<User, ApplicationUser, string>(provider.GetRequiredService<AuthDbContext>(), provider.GetRequiredService<IEntityMapper>()));

            services.AddScoped<IGenericRepository<Role, string>>(provider =>
                new GenericRepository<Role, ApplicationRole, string>(provider.GetRequiredService<AuthDbContext>(),provider.GetRequiredService<IEntityMapper>()));

            services.AddScoped<IGenericRepository<AuthService
                .Domain.Entities.Tenant, int>>(provider =>
                new GenericRepository<AuthService
                .Domain.Entities.Tenant, AuthService.Infrastructure.Persistance.Entities.Tenant, int>(provider.GetRequiredService<AuthDbContext>(),provider.GetRequiredService<IEntityMapper>()));

            services.AddScoped<IGenericRepository<AuthService
                .Domain.Entities.RefreshToken, int>>(provider =>
                new GenericRepository<AuthService
                .Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken, int>(provider.GetRequiredService<AuthDbContext>(), provider.GetRequiredService<IEntityMapper>()));

            // Specific repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<IUnitOfWork>(provider =>
            {
                var dbContext = provider.GetRequiredService<AuthDbContext>();

                return new UnitOfWork(
                    dbContext,
                    ctx => provider.GetRequiredService<IUserRepository>(),
                    ctx => provider.GetRequiredService<ITenantRepository>(),
                    ctx => provider.GetRequiredService<IRefreshTokenRepository>(),
                    ctx => provider.GetRequiredService<IRoleRepository>()
                );
            });

            return services;
        }
    }
}
