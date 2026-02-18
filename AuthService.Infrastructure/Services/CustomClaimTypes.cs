namespace AuthService.Infrastructure.Services
{
    public partial class TokenService
    {
        public class CustomClaimTypes
        {
            public const string TenantId = "tenantId";
            public const string TenantDomain = "tenantDomain";
            public const string TokenType = "token_type";
        }
    }
}