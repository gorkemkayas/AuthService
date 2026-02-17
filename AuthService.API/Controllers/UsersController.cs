using Asp.Versioning;
using AuthService.Application.Common;
using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.API.Controllers.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/admin/tenants/{tenantId:int}/[controller]")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        public UsersController(IUserService userService, IOptions<Application.Common.TokenOptions> tokenOptions, IRoleService roleService) : base(tokenOptions)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetForTenant(int tenantId, [FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
            var result = await _userService.GetUsersByTenantAsync(tenantId, page, pageSize);
            return FromServiceResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateForTenant(int tenantId, [FromBody] CreateTenantUserRequest request)
        {
            request.TenantId = tenantId;
            var result = await _userService.CreateTenantUserAsync(request);
            return FromServiceResult(result);
        }

        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> SetUserRoles(int tenantId, string userId, [FromBody] List<string> roles)
        {
            roles ??= new List<string>();

            var user = await _userService.GetUserByIdAsync(userId);
            if (user is null)
                return NotFound(new { success = false, message = "User not found" });

            // Tenant izolasyonu istiyorsan aç:
            // if (user.TenantId != tenantId)
            //     return BadRequest(new { success = false, message = "User does not belong to this tenant" });

            var currentRoles = await _roleService.GetUserRolesAsync(userId);

            var desired = roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var toRemove = currentRoles.Data.Except(desired, StringComparer.OrdinalIgnoreCase).ToList();
            var toAdd = desired.Except(currentRoles.Data, StringComparer.OrdinalIgnoreCase).ToList();

            if (toRemove.Count > 0)
            {
                var removeRes = await _userService.RemoveFromRolesAsync(user, toRemove);
                if (!removeRes.Success)
                {
                    var errors = string.Join(", ", removeRes.Message);
                    return BadRequest(new { success = false, message = $"Remove roles failed: {errors}" });
                }
            }

            if (toAdd.Count > 0)
            {
                var addRes = await _userService.AddToRolesAsync(user, toAdd);
                if (!addRes.Success)
                {
                    var errors = string.Join(", ", addRes.Message);
                    return BadRequest(new { success = false, message = $"Add roles failed: {errors}" });
                }
            }

            return Ok(new { success = true, message = "Roles updated" });
        }


        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(int tenantId, string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user is null)
                return NotFound(new { success = false, message = "User not found" });

            // Tenant izolasyonu istiyorsan aç:
            // if (user.TenantId != tenantId)
            //    return BadRequest(new { success = false, message = "User does not belong to this tenant" });

            var roles = await _roleService.GetUserRolesAsync(userId);

            return FromServiceResult(roles);
        }

    }
}