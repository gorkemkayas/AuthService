using AuthService.Infrastructure.Common;

namespace AuthService.Infrastructure.Persistance.Entities
{
    public class RefreshToken : IBaseEntity
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; } = false;

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceName { get; set; }

        public string ClientType { get; set; } = null!;
        public string? DeviceId { get; set; }

        public DateTime? RevokedAt { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }

        // IBaseEntity Implementations
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Properties
        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
