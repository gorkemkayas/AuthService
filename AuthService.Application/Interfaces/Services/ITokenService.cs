using AuthService.Application.Common;
using AuthService.Application.Dtos.Logout;
using AuthService.Application.Dtos.Refresh;
using AuthService.Application.Dtos.RefreshToken;
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
        Task<ServiceResult<CreateAdminUserTokenResponse>> CreateAdminUserTokenAsync(CreateAdminUserTokenRequest request);
        IEnumerable<RefreshTokenDto> GetActiveRefreshTokensByUserId(string userId);
        Task<RefreshTokenDto?> GetActiveRefreshTokenByDeviceNameAsync(string userId, string deviceName);
        Task<ServiceResult<RefreshResponse>> RefreshAsync(string? refreshToken, AuditInfo clientInformations, string clientType, string? deviceId);
        Task RevokeAllDevicesAsync(string userId, string clientIp);
        Task RevokeWebRefreshTokensAsync(string userId, AuditInfo clientInformations);
        Task<ServiceResult> RevokeDeviceRefreshTokensAsync(string userId, string clientType, string deviceId, AuditInfo clientInformations);
    }
}
