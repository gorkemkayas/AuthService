namespace AuthService.Application.Dtos.User
{
    public class CreateTenantUserTokenResponse
    {
        public string Token { get; set; } = null!;
        public string? RefreshToken { get; set; }
    }
    public class CreateAdminUserTokenResponse
    {
        public string Token { get; set; } = null!;
        public string? RefreshToken { get; set; }
    }
}
