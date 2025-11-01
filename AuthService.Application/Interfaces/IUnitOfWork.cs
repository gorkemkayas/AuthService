using AuthService.Application.Interfaces.Repositories;

namespace AuthService.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ITenantRepository Tenants { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
