using AuthService.API.Models;
using AuthService.Application.Common;
using AuthService.Application.Interfaces.Contexts;
using AuthService.Application.Results;
using AuthService.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly TokenOptions _tokenOptions;

        protected BaseController(IOptions<TokenOptions> tokenOptions)
        {
            _tokenOptions = tokenOptions.Value;
        }

        [NonAction]
        public IActionResult FromServiceResult(ServiceResult result)
        {
            if (result.Success)
                return Ok(ApiResult.Ok(result.Message ?? "Operation completed successfully."));

            return result.ErrorCode switch
            {
                ErrorCodes.ValidationError => BadRequest(ApiResult.Fail(result.Message!, result.ErrorCode)),
                ErrorCodes.Unauthorized => Unauthorized(ApiResult.Fail(result.Message!, result.ErrorCode)),
                ErrorCodes.Forbidden => Forbid(),
                ErrorCodes.NotFound => NotFound(ApiResult.Fail(result.Message!, result.ErrorCode)),
                ErrorCodes.Conflict => Conflict(ApiResult.Fail(result.Message!, result.ErrorCode)),
                ErrorCodes.AlreadyExists => Conflict(ApiResult.Fail(result.Message!, result.ErrorCode)),
                ErrorCodes.NotAcceptable => StatusCode(406, ApiResult.Fail(result.Message!, result.ErrorCode)),
                ErrorCodes.UnsupportedMediaType => StatusCode(415, ApiResult.Fail(result.Message!, result.ErrorCode)),
                ErrorCodes.TooManyRequests => StatusCode(429, ApiResult.Fail(result.Message!, result.ErrorCode)),
                ErrorCodes.TokenExpired => Unauthorized(ApiResult.Fail(result.Message!, result.ErrorCode)),
                _ => StatusCode(500, ApiResult.Fail(result.Message!, result.ErrorCode!))
            };
        }

        [NonAction]
        public IActionResult FromServiceResult<T>(ServiceResult<T> result)
        {
            if (result.Success)
                return Ok(ApiResult<T>.Ok(result.Data!, result.Message ?? "Operation completed successfully."));

            return result.ErrorCode switch
            {
                ErrorCodes.ValidationError => BadRequest(ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                ErrorCodes.Unauthorized => Unauthorized(ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                ErrorCodes.Forbidden => Forbid(),
                ErrorCodes.NotFound => NotFound(ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                ErrorCodes.Conflict => Conflict(ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                ErrorCodes.AlreadyExists => Conflict(ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                ErrorCodes.NotAcceptable => StatusCode(406, ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                ErrorCodes.UnsupportedMediaType => StatusCode(415, ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                ErrorCodes.TooManyRequests => StatusCode(429, ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                ErrorCodes.TokenExpired => Unauthorized(ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                _ => StatusCode(500, ApiResult<T>.Fail(result.Message!, result.ErrorCode!))
            };
        }

        protected void SetRefreshTokenCookie(string refreshToken)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api",
                Expires = DateTimeOffset.UtcNow.AddDays(_tokenOptions.WebRefreshTokenLifetimeDays)
            };
            Response.Cookies.Append("refreshToken", refreshToken, options);
        }

        protected AuditInfo GetClientInformations()
        {
            var ipAddress = ClientIpHelper.GetClientIp(HttpContext);
            var userAgent = Request.Headers["User-Agent"].ToString();
            var deviceName = DeviceParser.Parse(userAgent);

            return new AuditInfo(ipAddress, userAgent, deviceName);
        }

    }
}
