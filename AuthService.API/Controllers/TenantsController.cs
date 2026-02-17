using Asp.Versioning;
using AuthService.Application.Common;
using AuthService.Application.Dtos.Tenant;
using AuthService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.API.Controllers.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/admin/[controller]")]
    [Authorize(Roles = "SuperAdmin")] // or a policy like "AdminOnly"
    public class TenantsController : BaseController
    {
        private readonly ITenantService _tenantService;
        public TenantsController(ITenantService tenantService, IOptions<TokenOptions> tokenOptions) : base(tokenOptions)
        {
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _tenantService.GetAllTenantsAsync();
            return FromServiceResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _tenantService.GetTenantByIdAsync(id);
            return FromServiceResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTenantDto dto)
        {
            var result = await _tenantService.CreateNewTenantAsync(dto);
            return result.Success ? CreatedAtAction(nameof(Get), new { id = result.Data!.Id }, result) : BadRequest(result);
        }

        [HttpPost("{id:int}/enable")]
        public async Task<IActionResult> Enable(int id)
        {
            var result = await _tenantService.EnableTenantAsync(id);
            return FromServiceResult(result);
        }

        [HttpPost("{id:int}/disable")]
        public async Task<IActionResult> Disable(int id)
        {
            var result = await _tenantService.DeleteTenantAsync(id); // rename if you want disable
            return FromServiceResult(result);
        }
    }
}