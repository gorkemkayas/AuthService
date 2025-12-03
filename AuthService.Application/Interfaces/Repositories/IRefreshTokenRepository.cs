namespace AuthService.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<bool> DeleteRefreshTokenById(int id);
    }
}
