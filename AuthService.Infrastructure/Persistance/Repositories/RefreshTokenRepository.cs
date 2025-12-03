using AuthService.Application.Interfaces.Repositories;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.DbContexts;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class RefreshTokenRepository(AuthDbContext context, IEntityMapper mapper) : GenericRepository<AuthService.Domain.Entities.RefreshToken, AuthService.Infrastructure.Persistance.Entities.RefreshToken, int>(context, mapper), IRefreshTokenRepository
    {
        public async Task<bool> DeleteRefreshTokenById(int id)
        {
            var refreshToken = await FindAsync(id);

            if(refreshToken is null)
                return false;

            refreshToken.IsDeleted = true;
            refreshToken.UpdatedAt = DateTime.UtcNow;
            return true;

        }
    }
}