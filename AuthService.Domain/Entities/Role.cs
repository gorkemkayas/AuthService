using AuthService.Domain.Entities.Base;

namespace AuthService.Domain.Entities;

public class Role : BaseEntity<string>
{
    public string Name { get; set; } = null!;
}