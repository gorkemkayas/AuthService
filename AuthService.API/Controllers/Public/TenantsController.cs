using Asp.Versioning;
using AuthService.API.Models;
using AuthService.Application.Common;
using AuthService.Application.Dtos.Tenant;
using AuthService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.API.Controllers.Public
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/tenants")]
    public sealed class TenantsController : BaseController
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService, IOptions<TokenOptions> tokenOptions) : base(tokenOptions)
        {
            _tenantService = tenantService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterTenantRequest request)
        {
            var result = await _tenantService.RegisterTenantAsync(request);
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    ErrorCodes.ValidationError => BadRequest(ApiResult.Fail(result.Message!, result.ErrorCode!)),
                    ErrorCodes.AlreadyExists => Conflict(ApiResult.Fail(result.Message!, result.ErrorCode!)),
                    ErrorCodes.Conflict => Conflict(ApiResult.Fail(result.Message!, result.ErrorCode!)),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ApiResult.Fail(result.Message!, result.ErrorCode ?? ErrorCodes.Unexpected))
                };
            }

            return StatusCode(StatusCodes.Status201Created, result.Data);
        }
    }
}
