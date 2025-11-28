using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class TenantRepository : GenericRepository<AuthService.Domain.Entities.Tenant, AuthService.Infrastructure.Persistance.Entities.Tenant, int>, ITenantRepository
    {
        public TenantRepository(AuthDbContext context, IEntityMapper mapper)
            : base(context, mapper)
        {
        }

        public Task<Tenant?> GetByIdAsync(int id) => base.GetByIdAsync(id);
        public async Task<int> GetActiveTenantCountAsync(CancellationToken cancellationToken = default) => await _context.Tenants.CountAsync(t => t.IsActive, cancellationToken);
        public async Task<int> GetInactiveTenantCountAsync(CancellationToken cancellationToken = default) => await _context.Tenants.CountAsync(t => !t.IsActive, cancellationToken);
        public async Task<int> GetTotalTenantCountAsync(CancellationToken cancellationToken = default) => await _context.Tenants.CountAsync(cancellationToken);
        public IEnumerable<string> GetAllDomainAddresses() => _context.Tenants.Select(t => t.Domain).AsEnumerable();

    }
}
