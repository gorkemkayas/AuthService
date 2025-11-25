using AuthService.Domain.Entities.Base;

namespace AuthService.Domain.Entities;
public class Tenant : BaseEntity<int>
{
    public string Name { get; set; } = null!;  // Mağaza/firma adı
    public string Domain { get; set; } = null!; // Opsiyonel, mağaza alan adı
    public bool IsActive { get; set; } = true;
    public bool IsSystem { get; set; } = false;

    public string Email { get; set; } = null!; // Emails will be as 'name@kayas.com'
    public string HashedPassword { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new List<User>();
}
