using Asp.Versioning;
using AuthService.Application.Common;
using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
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
        public UsersController(IUserService userService, IOptions<TokenOptions> tokenOptions) : base(tokenOptions)
        {
            _userService = userService;
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
        public async Task<IActionResult> AssignRoles(int tenantId, string userId, [FromBody] List<string> roles)
        {
            var result = await _userService.AssignRolesToUserAsync(userId, tenantId, roles);
            return FromServiceResult(result);
        }


    }
}