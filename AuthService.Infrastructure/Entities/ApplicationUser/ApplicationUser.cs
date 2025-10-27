using AuthService.Infrastructure.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser, IBaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        [NotMapped]
        public string FullName => $"{Name} {Surname}";
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }

        // IBaseEntity Implementations
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Properties
        public ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}
