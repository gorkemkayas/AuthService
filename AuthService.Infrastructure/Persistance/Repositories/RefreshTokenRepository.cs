using AuthService.Application.Interfaces.Repositories;
using AuthService.Infrastructure.Persistance.Entities;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class RefreshTokenRepository(GenericRepository<RefreshToken> genericRepository) : IRefreshTokenRepository
    {
        private readonly GenericRepository<RefreshToken> _genericRepository = genericRepository;
    }
}
