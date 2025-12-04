using Asp.Versioning;
using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/[controller]")]
    public class AuthController : BaseController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        public AuthController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest loginUserRequest)
        {
            var result = await _userService.LoginAsync(loginUserRequest);
            if (!result.Success)
                return FromServiceResult(result);

            var tokenResult = await _tokenService.CreateTenantUserTokenAsync(new CreateTenantUserTokenRequest
            {
                UserId = result.Data!.UserId,
                Email = result.Data.Email,
                TenantId = result.Data.TenantId,
                TenantDomain = result.Data.TenantDomain,
            });

            return FromServiceResult(tokenResult);

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateTenantUserRequest request)
        {
            var result = await _userService.CreateTenantUserAsync(request);
            return FromServiceResult(result);
        }
    }
}
