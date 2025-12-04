namespace AuthService.Application.Dtos.User
{
    public class CreateTenantUserRequest
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int TenantId { get; set; }
        public bool IsPersistent { get; set; } = false;

    }

}
