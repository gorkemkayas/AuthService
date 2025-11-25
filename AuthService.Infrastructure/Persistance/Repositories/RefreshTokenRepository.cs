using AuthService.Application.Interfaces.Repositories;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.DbContexts;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class RefreshTokenRepository(AuthDbContext context, IEntityMapper mapper) : GenericRepository<AuthService.Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken, int>(context, mapper), IRefreshTokenRepository
    {
    }
}