namespace AuthService.Application.Dtos.User
{
    public class CreateTenantUserResponse
    {
        public Guid TenantUserId { get; set; }
        public bool RequiresEmailVerification { get; set; }
    }
}