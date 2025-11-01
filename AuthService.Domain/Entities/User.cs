using AuthService.Domain.Entities.Base;

namespace AuthService.Domain.Entities;

public class User : BaseEntity<string>
{
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string FullName => $"{Name} {Surname}";
    public string Email { get; set; } = null!;
    public int TenantId { get; set; }
    public string PasswordSalt { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
}
