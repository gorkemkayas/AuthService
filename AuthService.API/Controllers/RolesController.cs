using Asp.Versioning;
using AuthService.Application.Interfaces.Services;
using AuthService.Infrastructure.Persistance.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin/[controller]")]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public class RolesController : BaseController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRoleService _roleService;

        public RolesController(RoleManager<ApplicationRole> roleManager, IRoleService roleService, IOptions<Application.Common.TokenOptions> tokenOptions) : base(tokenOptions)
        {
            _roleManager = roleManager;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return FromServiceResult(roles);
        }
    }
}
