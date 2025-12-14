using AuthService.API.Models;
using AuthService.Application.Common;

namespace AuthService.API.Middlewares
{
    public class ClientTypeMiddleware
    {
        private readonly RequestDelegate _next;
        public ClientTypeMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var clientType = context.Request.Headers[ClientContextKeys.ClientType].ToString().ToLowerInvariant();
            var deviceId = context.Request.Headers[ClientContextKeys.DeviceId].ToString();

            if (string.IsNullOrWhiteSpace(clientType))
                clientType = ClientTypes.Web; // default

            if (clientType != ClientTypes.Web && string.IsNullOrWhiteSpace(deviceId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync<ApiResult>(ApiResult.Fail("DeviceId is required for this client type.",ErrorCodes.ValidationError));
                return;
            }

            context.Items[ClientContextKeys.ClientType] = clientType;
            context.Items[ClientContextKeys.DeviceId] = string.IsNullOrWhiteSpace(deviceId) ? null : deviceId;

            await _next(context);
        }
    }

}
