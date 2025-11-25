namespace AuthService.Application.Dtos.User
{
    public class CreateUserDto
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public int TenantId { get; set; }

    }
    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
    }
    public class UserDto
    {
        public string Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string FullName => $"{Name} {Surname}";
        public string Email { get; set; } = null!;
        public int TenantId { get; set; }

    }

}
