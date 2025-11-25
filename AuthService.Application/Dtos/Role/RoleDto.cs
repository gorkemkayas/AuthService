namespace AuthService.Application.Dtos.Role
{
    public class RoleDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
