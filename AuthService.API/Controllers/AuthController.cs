using Asp.Versioning;
using AuthService.API.Models;
using AuthService.Application.Common;
using AuthService.Application.Dtos.Refresh;
using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces.Contexts;
using AuthService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/[controller]")]
    public class AuthController : BaseController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IClientContext _clientContext;
        public AuthController(ITokenService tokenService, IUserService userService, IClientContext clientContext, IOptions<TokenOptions> tokenOptions) : base(tokenOptions)
        {
            _tokenService = tokenService;
            _userService = userService;
            _clientContext = clientContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest loginUserRequest)
        {
            var result = await _userService.LoginAsync(loginUserRequest);
            if (!result.Success)
                return FromServiceResult(result);

            var clientInformations = GetClientInformations();


            var tokenResult = await _tokenService.CreateTenantUserTokenAsync(new CreateTenantUserTokenRequest
            {
                UserId = result.Data!.UserId,
                Email = result.Data.Email,
                TenantId = result.Data.TenantId,
                TenantDomain = result.Data.TenantDomain,
                IpAddress = clientInformations.IpAddress,
                UserAgent = clientInformations.UserAgent,
                DeviceName = clientInformations.DeviceName
            });

            if (!tokenResult.Success)
                return FromServiceResult(tokenResult);

            if (_clientContext.ClientType == ClientTypes.Web)
            {
                SetRefreshTokenCookie(tokenResult.Data!.RefreshToken!);
                var tenantResponse = new CreateTenantUserTokenResponse()
                {
                    Token = tokenResult.Data!.Token,
                };
                return Ok(ApiResult<CreateTenantUserTokenResponse>.Ok(tenantResponse, tokenResult.Message));
            }


            return FromServiceResult(tokenResult);

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateTenantUserRequest request)
        {
            var result = await _userService.CreateTenantUserAsync(request);
            return FromServiceResult(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest? refreshRequest)
        {
            var userInformations = GetClientInformations();
            string? refreshToken;

            if (_clientContext.ClientType == ClientTypes.Web)
            {
                refreshToken = Request.Cookies["refreshToken"];
            }
            else
            {
                refreshToken = refreshRequest?.RefreshToken;
            }

            var tokenResult = await _tokenService.RefreshAsync(refreshToken, userInformations);
            if (!tokenResult.Success) return FromServiceResult(tokenResult);

            if (_clientContext.ClientType == ClientTypes.Web)
            {
                SetRefreshTokenCookie(tokenResult.Data!.RefreshToken!);
                var tenantResponse = new CreateTenantUserTokenResponse()
                {
                    Token = tokenResult.Data!.AccessToken!,
                };
                return Ok(ApiResult<CreateTenantUserTokenResponse>.Ok(tenantResponse, tokenResult.Message));
            }

            return FromServiceResult(tokenResult);
        }
    }
}
