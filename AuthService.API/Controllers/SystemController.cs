using Asp.Versioning;
using AuthService.API.Models;
using AuthService.Application.Common;
using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces.Contexts;
using AuthService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SystemController : BaseController
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IClientContext _clientContext;
        public SystemController(IOptions<TokenOptions> tokenOptions, ITokenService tokenService, IUserService userService, IClientContext clientContext) : base(tokenOptions)
        {
            _tokenService = tokenService;
            _userService = userService;
            _clientContext = clientContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest loginUserRequest)
        {
            var result = await _userService.LoginAsSystemAdminAsync(loginUserRequest);
            if (!result.Success)
                return FromServiceResult(result);

            var clientInformations = GetClientInformations();


            var tokenResult = await _tokenService.CreateAdminUserTokenAsync(new CreateAdminUserTokenRequest
            {
                UserId = result.Data!.UserId,
                Email = result.Data.Email,
                IpAddress = clientInformations.IpAddress,
                UserAgent = clientInformations.UserAgent,
                DeviceName = clientInformations.DeviceName,
                ClientType = _clientContext.ClientType,
                DeviceId = _clientContext.DeviceId
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
    }
}
