using AuthService.Application.Interfaces.Repositories;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.DbContexts;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class TenantRepository(AuthDbContext context, IEntityMapper mapper) : GenericRepository<AuthService.Domain.Entities.Tenant, AuthService.Infrastructure.Persistance.Entities.Tenant, int>(context,mapper), ITenantRepository
    {
    }
}
