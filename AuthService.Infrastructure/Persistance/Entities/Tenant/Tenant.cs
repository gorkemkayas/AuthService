namespace AuthService.Infrastructure.Persistance.Entities
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSystem { get; set; } = false;

        // IBaseEntity Implementations
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation Properties
        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
