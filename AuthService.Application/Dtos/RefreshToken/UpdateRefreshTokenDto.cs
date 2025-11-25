namespace AuthService.Application.Dtos.RefreshToken;

// Token güncelleme (revoked veya expiry update)
public class UpdateRefreshTokenDto
{
    public bool? IsRevoked { get; set; }
    public DateTime? Expires { get; set; }
}

