using AuthService.Domain.Entities.Base;

namespace AuthService.Domain.Entities;
public class Tenant : BaseEntity<int>
{
    public string Name { get; set; } = null!;  // Mağaza/firma adı
    public string Domain { get; set; } = null!; // Opsiyonel, mağaza alan adı
    public bool IsActive { get; set; } = true;
    public bool IsSystem { get; set; } = false;
    public ICollection<User> Users { get; set; } = new List<User>();
}
