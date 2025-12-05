using AuthService.Application.Common;
using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Results;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Common;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityMapper _entityMapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEntityMapper entityMapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _entityMapper = entityMapper;
        }
        public async Task<ServiceResult<UserDto>> CreateTenantUserAsync(CreateTenantUserRequest request)
        {
            var userExists = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (userExists is not null)
            {
                return ServiceResult<UserDto>.Fail("User with the given email already exists.", ErrorCodes.AlreadyExists);
            }
            var isTenantExists = await _unitOfWork.Tenants.ExistsAsync(request.TenantId);
            if (!isTenantExists)
                return ServiceResult<UserDto>.Fail("Tenant not found.", ErrorCodes.NotFound);

            var tenant = await _unitOfWork.Tenants.GetByIdAsync(request.TenantId);
            if (tenant == null)
                return ServiceResult<UserDto>.Fail("Tenant not found.", ErrorCodes.NotFound);

            var mappedTenant = _entityMapper.MapToData<AuthService.Domain.Entities.Tenant,AuthService.Infrastructure.Persistance.Entities.Tenant>(tenant);
            var newUser = new ApplicationUser
            {
                UserName = GeneratorHelper.GenerateUsername(request.Name, request.Surname),
                Email = request.Email,
                Name = request.Name,
                Surname = request.Surname,
                TenantId = request.TenantId,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userManager.CreateAsync(newUser, request.Password);

            if (createdUser is null || !createdUser.Succeeded)
            {
                var errors = createdUser?.Errors.Select(e => e.Description).ToArray() ?? Array.Empty<string>();
                return ServiceResult<UserDto>.Fail($"Failed to create user. {string.Join(", ", errors)}", ErrorCodes.Unexpected);

            }
            var userDto = new UserDto
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Surname = newUser.Surname,
                Email = newUser.Email,
                TenantId = newUser.TenantId,
                CreatedAt = newUser.CreatedAt
            };
            return ServiceResult<UserDto>.Ok(userDto, "User created successfully.");
        }
        public async Task<ServiceResult<UserDto>> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user is null)
            {
                return ServiceResult<UserDto>.Fail("User not found.", ErrorCodes.NotFound);
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                TenantId = user.TenantId,
                CreatedAt = user.CreatedAt
            };
            return ServiceResult<UserDto>.Ok(userDto, "User retrieved successfully.");
        }
        public async Task<ServiceResult<UserDto>> GetUserByIdAsync(string id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is null)
            {
                return ServiceResult<UserDto>.Fail("User not found.", ErrorCodes.NotFound);
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                TenantId = user.TenantId,
                CreatedAt = user.CreatedAt
            };
            return ServiceResult<UserDto>.Ok(userDto, "User retrieved successfully.");
        }
        public async Task<ServiceResult> DeleteUserAsync(string id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is null)
            {
                return ServiceResult.Fail("User not found.", ErrorCodes.NotFound);
            }

            await _unitOfWork.Users.DeleteUserById(id);
            var effectedOnes = await _unitOfWork.SaveChangesAsync();
          
            if(effectedOnes == 0)
            {
                return ServiceResult.Fail("Failed to delete user.", ErrorCodes.Unexpected);
            }

            return ServiceResult.Ok("User deleted successfully.");
        }
        public async Task<ServiceResult> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is null)
                return ServiceResult.Fail("User not found.", ErrorCodes.NotFound);

            var appUser = await _userManager.FindByIdAsync(user.Id);
            appUser!.Name = updateUserDto.Name!;
            appUser.Surname = updateUserDto.Surname!;
            appUser.Email = updateUserDto.Email;
            var result = await _userManager.UpdateAsync(appUser);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return ServiceResult.Fail($"Failed to update user. {string.Join(", ", errors)}", ErrorCodes.Unexpected);
            }
            return ServiceResult.Ok("User updated successfully.");
        }
        public async Task<ServiceResult> ChangeUserPasswordAsync(string id, ChangeUserPasswordDto changePasswordDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is null)
                return ServiceResult.Fail("User not found.", ErrorCodes.NotFound);

            var appUser = await _userManager.FindByIdAsync(user.Id);
            var result = await _userManager.ChangePasswordAsync(appUser!, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return ServiceResult.Fail($"Failed to change password. {string.Join(", ", errors)}", ErrorCodes.Unexpected);
            }
            return ServiceResult.Ok("Password changed successfully.");
        }
        public async Task<ServiceResult<int>> GetUserCountByTenantIdAsync(int tenantId)
        {
            var count = await _unitOfWork.Users.GetCountByTenantIdAsync(tenantId);
            return ServiceResult<int>.Ok(count, "User count retrieved successfully.");
        }
        public async Task<ServiceResult<bool>> CheckUserExistsAsync(string id)
        {
            var exists = await _unitOfWork.Users.ExistsAsync(id);
            return ServiceResult<bool>.Ok(exists, "User existence check completed successfully.");
        }
        public async Task<ServiceResult<bool>> CheckUserExistsByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            var exists = user is not null;
            return ServiceResult<bool>.Ok(exists, "User existence check by email completed successfully.");
        }
        public async Task<ServiceResult> ResetUserPasswordAsync(string email, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user is null)
                return ServiceResult.Fail("User not found.", ErrorCodes.NotFound);
            var appUser = await _userManager.FindByIdAsync(user.Id);
            var token = await _userManager.GeneratePasswordResetTokenAsync(appUser!);
            var result = await _userManager.ResetPasswordAsync(appUser!, token, newPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return ServiceResult.Fail($"Failed to reset password. {string.Join(", ", errors)}", ErrorCodes.Unexpected);
            }
            return ServiceResult.Ok("Password reset successfully.");
        }

        public async Task<ServiceResult<LoginUserResponse>> LoginAsync(LoginUserRequest loginUserRequest)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(loginUserRequest.Email);
            if (user is null)
                return ServiceResult<LoginUserResponse>.Fail("User not found.", ErrorCodes.NotFound);

            var appUser = await _userManager.FindByIdAsync(user.Id);

            if (await _userManager.IsLockedOutAsync(appUser!))
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(appUser!);
                var minutesLeft = (lockoutEnd!.Value - DateTimeOffset.UtcNow).TotalMinutes;
                return ServiceResult<LoginUserResponse>.Fail(
                    $"Account is locked. Try again in {Math.Ceiling(minutesLeft)} minutes.",
                    ErrorCodes.Unauthorized
                );
            }
            var passwordValid = await _userManager.CheckPasswordAsync(appUser!, loginUserRequest.Password);
            if (!passwordValid)
            {
                await _userManager.AccessFailedAsync(appUser!);  // lockout sayaç artır
                var accessFailedCount = await _userManager.GetAccessFailedCountAsync(appUser!);
                var max = _userManager.Options.Lockout.MaxFailedAccessAttempts;

                return ServiceResult<LoginUserResponse>.Fail(
                    $"Invalid password. {max - accessFailedCount} attempts remaining before lockout.",
                    ErrorCodes.Unauthorized
                );
            }

            await _userManager.ResetAccessFailedCountAsync(appUser!);

            var tenant = await _unitOfWork.Tenants.FindAsync(user.TenantId);

            return ServiceResult<LoginUserResponse>.Ok(new LoginUserResponse
            {
                UserId = appUser!.Id,
                Email = appUser.Email!,
                TenantId = tenant!.Id,
                TenantDomain = appUser.Tenant.Domain ?? string.Empty
            }, "Login successful.");
        }
    }
}
