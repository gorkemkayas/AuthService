using AuthService.Application.Common;
using AuthService.Application.Dtos.Logout;
using AuthService.Application.Dtos.User;
using AuthService.Application.Results;

namespace AuthService.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ServiceResult<UserDto>> CreateTenantUserAsync(CreateTenantUserRequest request);
        Task<ServiceResult<UserDto>> GetUserByEmailAsync(string email);
        Task<ServiceResult<UserDto>> GetUserByIdAsync(string id);
        Task<ServiceResult> DeleteUserAsync(string id);
        Task<ServiceResult> UpdateUserAsync(string id, UpdateUserDto updateUserDto);
        Task<ServiceResult> ChangeUserPasswordAsync(string id, ChangeUserPasswordDto changePasswordDto);
        Task<ServiceResult<int>> GetUserCountByTenantIdAsync(int tenantId);
        Task<ServiceResult<bool>> CheckUserExistsAsync(string id);
        Task<ServiceResult<bool>> CheckUserExistsByEmailAsync(string email);
        Task<ServiceResult> ResetUserPasswordAsync(string email, string newPassword);
        Task<ServiceResult<LoginUserResponse>> LoginAsync(LoginUserRequest loginUserRequest);
        Task<ServiceResult> LogoutAsync(string userId, AuditInfo auditInfo, string clientType, string? deviceId);
        Task<ServiceResult> LogoutAllDevicesAsync(string userId, AuditInfo auditInfo);
    }
}
