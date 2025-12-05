namespace AuthService.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IGenericRepository<Domain.Entities.RefreshToken, int>
    {
        Task<bool> DeleteRefreshTokenById(int id);
        IEnumerable<AuthService.Domain.Entities.RefreshToken> GetActiveRefreshTokensByUserId(string userId);
        Task<AuthService.Domain.Entities.RefreshToken?> GetActiveRefreshTokenByUserDeviceAsync(string userId, string deviceName);
        Task<AuthService.Domain.Entities.RefreshToken?> GetLastRefreshTokenByDeviceAsync(string userId, string deviceName);
    }
}
