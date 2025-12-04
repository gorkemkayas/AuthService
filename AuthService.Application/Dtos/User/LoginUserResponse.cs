namespace AuthService.Application.Dtos.User
{
    public class LoginUserResponse
    {
        //string userId, string email, string tenantId, string tenantDomain
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int TenantId { get; set; }
        public string TenantDomain { get; set; } = null!;
    }
}
