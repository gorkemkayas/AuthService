using AuthService.API.Models;
using AuthService.Application.Common;
using AuthService.Application.Results;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult FromServiceResult(ServiceResult result)
        {
            if (result.Success) return Ok(ApiResult.Ok(result.Message ?? "Operation completed successfully."));

            return result.ErrorCode switch
            {
                ErrorCodes.NotFound => NotFound(ApiResult.Fail(result.Message!, result.ErrorCode)),
                ErrorCodes.ValidationError => BadRequest(ApiResult.Fail(result.Message!, result.ErrorCode)),
                ErrorCodes.Unauthorized => Unauthorized(ApiResult.Fail(result.Message!, result.ErrorCode)),
                _ => StatusCode(500, ApiResult.Fail(result.Message!, result.ErrorCode!))
            };
        }

        [NonAction]
        public IActionResult FromServiceResult<T>(ServiceResult<T> result)
        {
            if (result.Success) return Ok(ApiResult<T>.Ok(result.Data!, result.Message ?? "Operation completed successfully."));

            return result.ErrorCode switch
            {
                ErrorCodes.NotFound => NotFound(ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                ErrorCodes.ValidationError => BadRequest(ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                ErrorCodes.Unauthorized => Unauthorized(ApiResult<T>.Fail(result.Message!, result.ErrorCode!)),
                _ => StatusCode(500, ApiResult<T>.Fail(result.Message!, result.ErrorCode!))
            };
        }
    }
}
