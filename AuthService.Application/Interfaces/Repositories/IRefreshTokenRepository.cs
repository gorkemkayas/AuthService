namespace AuthService.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IGenericRepository<Domain.Entities.RefreshToken, int>
    {
        Task<bool> DeleteRefreshTokenById(int id);
        Task<AuthService.Domain.Entities.RefreshToken?> FindByTokenAsync(string refreshToken,bool isTracked = true);
        IEnumerable<AuthService.Domain.Entities.RefreshToken> GetActiveRefreshTokensByUserId(string userId);
        Task<AuthService.Domain.Entities.RefreshToken?> GetActiveRefreshTokenByUserDeviceAsync(string userId, string deviceName);
        Task<AuthService.Domain.Entities.RefreshToken?> GetActiveRefreshTokenByDeviceAsync(string userId, string deviceName);
        Task<AuthService.Domain.Entities.RefreshToken?> GetActiveRefreshTokenByClientTypeAsync(string userId, string clientType);
        Task<AuthService.Domain.Entities.RefreshToken?> GetActiveRefreshTokenByDeviceIdAsync(string userId, string clientType, string deviceId);
        Task<AuthService.Domain.Entities.RefreshToken?> GetActiveWebRefreshTokenAsync(string userId, bool isTracked = true);
        Task<AuthService.Domain.Entities.RefreshToken?> GetActiveDeviceRefreshTokenAsync(string userId, string clientType, string deviceId, bool isTracked = true);
    }
}
