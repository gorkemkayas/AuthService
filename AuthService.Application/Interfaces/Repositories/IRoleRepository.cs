using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Repositories
{
    public interface IRoleRepository : IGenericRepository<Role, string>
    {
        Task<Role?> GetByNameAsync(string name);
        Task<Role?> GetByIdAsync(string id);
        Task<IList<Role>> GetAllRolesAsync();
        Task<bool> CreateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(string id);
    }
}
