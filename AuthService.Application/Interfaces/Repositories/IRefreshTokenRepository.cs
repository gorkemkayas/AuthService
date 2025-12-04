namespace AuthService.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IGenericRepository<Domain.Entities.RefreshToken, int>
    {
        Task<bool> DeleteRefreshTokenById(int id);
    }
}
