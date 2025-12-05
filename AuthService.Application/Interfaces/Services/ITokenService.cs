using AuthService.Application.Dtos.User;
using AuthService.Application.Results;
using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateAdminToken(string adminId, string email);
        string CreateTenantToken(string userId, string email, string tenantId, string tenantDomain);
        Task<ServiceResult<CreateTenantUserTokenResponse>> CreateTenantUserTokenAsync(CreateTenantUserTokenRequest request);
        IEnumerable<RefreshToken> GetActiveRefreshTokensByUserId(string userId);
        Task<RefreshToken?> GetActiveRefreshTokenByDeviceNameAsync(string userId, string deviceName);
    }
}
