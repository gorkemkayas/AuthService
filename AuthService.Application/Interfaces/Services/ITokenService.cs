using AuthService.Application.Dtos.User;
using AuthService.Application.Results;

namespace AuthService.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateAdminToken(string adminId, string email);
        string CreateTenantToken(string userId, string email, string tenantId, string tenantDomain);
        Task<ServiceResult<CreateTenantUserTokenResponse>> CreateTenantUserTokenAsync(CreateTenantUserTokenRequest request);
    }
}
