using AuthService.Application.Common;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Results;
using AuthService.Infrastructure.Persistance.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ServiceResult> CreateRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
                return ServiceResult.Fail("Role already exists.", ErrorCodes.Unexpected);
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return ServiceResult.Fail($"Failed to create role. {string.Join(", ", errors)}", ErrorCodes.Unexpected);
            }
            return ServiceResult.Ok("Role created successfully.");
        }
        public async Task<ServiceResult> AssignRoleToUserAsync(string userId, string roleName)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
                return ServiceResult.Fail("User not found.", ErrorCodes.NotFound);
            var appUser = await _userManager.FindByIdAsync(user.Id);
            var result = await _userManager.AddToRoleAsync(appUser!, roleName);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return ServiceResult.Fail($"Failed to assign role. {string.Join(", ", errors)}", ErrorCodes.Unexpected);
            }
            return ServiceResult.Ok("Role assigned successfully.");
        }
        public async Task<ServiceResult> RemoveRoleFromUserAsync(string userId, string roleName)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
                return ServiceResult.Fail("User not found.", ErrorCodes.NotFound);
            var appUser = await _userManager.FindByIdAsync(user.Id);
            var result = await _userManager.RemoveFromRoleAsync(appUser!, roleName);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return ServiceResult.Fail($"Failed to remove role. {string.Join(", ", errors)}", ErrorCodes.Unexpected);
            }
            return ServiceResult.Ok("Role removed successfully.");
        }
        public async Task<ServiceResult<IList<string>>> GetUserRolesAsync(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
                return ServiceResult<IList<string>>.Fail("User not found.", ErrorCodes.NotFound);
            var appUser = await _userManager.FindByIdAsync(user.Id);
            var roles = await _userManager.GetRolesAsync(appUser!);
            return ServiceResult<IList<string>>.Ok(roles, "User roles retrieved successfully.");
        }
    }
}
