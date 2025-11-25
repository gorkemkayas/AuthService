namespace AuthService.Application.Dtos.RefreshToken;

// Token creation genellikle backend tarafından yapılır
public class CreateRefreshTokenDto
{
    public string UserId { get; set; } = null!;
    public DateTime Expires { get; set; }
}

