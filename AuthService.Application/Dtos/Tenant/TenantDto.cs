namespace AuthService.Application.Dtos.Tenant
{
    public class TenantDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string Name { get; set; } = null!;  // Mağaza/firma adı
        public string Domain { get; set; } = null!; // Opsiyonel, mağaza alan adı

        public string Email { get; set; } = null!; // Emails will be as 'name@kayas.com'
        public bool IsActive { get; set; } = true;
        public bool IsSystem { get; set; } = false;
    }
}
