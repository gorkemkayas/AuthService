using AuthService.Application.Interfaces.Repositories;
using AuthService.Infrastructure.Persistance.Entities;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class TenantRepository(GenericRepository<Tenant> genericRepository) : ITenantRepository
    {
        private readonly GenericRepository<Tenant> _genericRepository = genericRepository;
    }
}
