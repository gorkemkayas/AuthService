using AuthService.Domain.Entities.Base;

namespace AuthService.Domain.Entities;

public class RefreshToken : BaseEntity<int>
{
    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } = false;

    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}
