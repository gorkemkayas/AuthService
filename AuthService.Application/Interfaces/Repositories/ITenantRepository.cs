using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Repositories
{
    public interface ITenantRepository : IGenericRepository<Tenant, int>
    {
        Task<Tenant?> GetByIdAsync(int id);
        IEnumerable<string> GetAllDomainAddresses();
        Task<int> GetActiveTenantCountAsync(CancellationToken cancellationToken = default);
        Task<int> GetInactiveTenantCountAsync(CancellationToken cancellationToken = default);
        Task<int> GetTotalTenantCountAsync(CancellationToken cancellationToken = default);
    }
}
