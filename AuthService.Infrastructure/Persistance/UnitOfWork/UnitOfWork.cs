using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Infrastructure.Persistance.DbContexts;

namespace AuthService.Infrastructure.Persistance.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _dbContext;
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<ITenantRepository> _tenantRepository;
        private readonly Lazy<IRefreshTokenRepository> _refreshTokenRepository;
        public UnitOfWork(AuthDbContext context, Func<AuthDbContext,IUserRepository> userRepository, Func<AuthDbContext,ITenantRepository> tenantRepository, Func<AuthDbContext,IRefreshTokenRepository> refreshTokenRepository)
        {
            _userRepository = new Lazy<IUserRepository>(() => userRepository(context));
            _tenantRepository = new Lazy<ITenantRepository>(() => tenantRepository(context));
            _refreshTokenRepository = new Lazy<IRefreshTokenRepository>(() => refreshTokenRepository(context));
            _dbContext = context;
        }
        public IUserRepository Users => _userRepository.Value;
        public ITenantRepository Tenants => _tenantRepository.Value;
        public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository.Value;
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
