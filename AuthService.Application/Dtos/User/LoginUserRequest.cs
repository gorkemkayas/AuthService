namespace AuthService.Application.Dtos.User
{
    public class LoginUserRequest
    {
        public string Email { get; set; }= null!;
        public string Password { get; set; } = null!;
        public bool IsPersistent { get; set; } = false;
    }
}
