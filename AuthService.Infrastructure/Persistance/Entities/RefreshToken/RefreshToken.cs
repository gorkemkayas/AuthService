using AuthService.Infrastructure.Common;

namespace AuthService.Infrastructure.Persistance.Entities
{
    public class RefreshToken : IBaseEntity
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; } = false;

        // IBaseEntity Implementations
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Properties
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
