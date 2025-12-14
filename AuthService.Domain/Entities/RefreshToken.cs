using AuthService.Domain.Entities.Base;

namespace AuthService.Domain.Entities;

public class RefreshToken : BaseEntity<int>
{
    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } = false;

    public string UserId { get; set; } = null!;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceName { get; set; }

    public string ClientType { get; set; } = null!;
    public string? DeviceId { get; set; }

    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public User User { get; set; } = null!;
}
