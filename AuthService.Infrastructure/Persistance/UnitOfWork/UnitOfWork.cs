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
        private readonly Lazy<IRoleRepository> _roleRepository;
        public UnitOfWork(AuthDbContext context, Func<AuthDbContext,IUserRepository> userRepository, Func<AuthDbContext,ITenantRepository> tenantRepository, Func<AuthDbContext,IRefreshTokenRepository> refreshTokenRepository, Func<AuthDbContext,IRoleRepository> roleRepository)
        {
            _userRepository = new Lazy<IUserRepository>(() => userRepository(context));
            _tenantRepository = new Lazy<ITenantRepository>(() => tenantRepository(context));
            _refreshTokenRepository = new Lazy<IRefreshTokenRepository>(() => refreshTokenRepository(context));
            _roleRepository = new Lazy<IRoleRepository>(() => roleRepository(context));
            _dbContext = context;
        }
        public IUserRepository Users => _userRepository.Value;
        public ITenantRepository Tenants => _tenantRepository.Value;
        public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository.Value;
        public IRoleRepository Roles => _roleRepository.Value;
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
