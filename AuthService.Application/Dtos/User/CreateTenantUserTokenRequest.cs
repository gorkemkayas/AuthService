namespace AuthService.Application.Dtos.User
{
    public class CreateTenantUserTokenRequest
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int TenantId { get; set; }
        public string TenantDomain { get; set; } = null!;

        // tracking için eklendi.
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
        public string DeviceName { get; set; } = null!;
    }
}
