using AuthService.Infrastructure.Common;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Persistance.Entities
{
    public class ApplicationRole : IdentityRole,IBaseEntity
    {
        public string Description { get; set; } // Opsiyonel açıklama

        // IBaseEntity Implementations
        public DateTime CreatedAt {get; set;}
        public DateTime? UpdatedAt { get; set;}
        public bool IsDeleted {  get; set;}
    }
}
