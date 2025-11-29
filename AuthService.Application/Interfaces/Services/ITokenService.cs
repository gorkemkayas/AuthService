namespace AuthService.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateAdminToken(string adminId, string email);
        string CreateTenantToken(string userId, string email, string tenantId, string tenantDomain);
        string CreateTenantUserToken(string userId, string email, string tenantId, string tenantDomain);
    }

}
