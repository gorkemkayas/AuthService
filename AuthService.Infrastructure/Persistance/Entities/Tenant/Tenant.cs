using AuthService.Infrastructure.Common;

namespace AuthService.Infrastructure.Persistance.Entities
{
    public class Tenant :IBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Domain { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSystem { get; set; } = false;

        // IBaseEntity Implementations
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public string Email { get; set; } = null!; // Emails will be as 'name@kayas.com'
        public string HashedPassword { get; set; } = "0000000000";

        // Navigation Properties
        public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
    }
}
