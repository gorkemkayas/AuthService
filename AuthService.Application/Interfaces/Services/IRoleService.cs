using AuthService.Application.Dtos.Role;
using AuthService.Application.Results;
using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ServiceResult> AssignRoleToUserAsync(string userId, string roleName);
        Task<ServiceResult> RemoveRoleFromUserAsync(string userId, string roleName);
        Task<ServiceResult<IList<string>>> GetUserRolesAsync(string userId);
        Task<ServiceResult<Role>> CreateRoleAsync(Role role);
        Task<ServiceResult<IEnumerable<RoleDto>>> GetAllRolesAsync();
    }

}
