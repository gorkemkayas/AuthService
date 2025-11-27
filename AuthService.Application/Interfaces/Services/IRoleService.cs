using AuthService.Application.Results;

namespace AuthService.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ServiceResult> AssignRoleToUserAsync(string userId, string roleName);
        Task<ServiceResult> RemoveRoleFromUserAsync(string userId, string roleName);
        Task<ServiceResult<IList<string>>> GetUserRolesAsync(string userId);
    }

}
