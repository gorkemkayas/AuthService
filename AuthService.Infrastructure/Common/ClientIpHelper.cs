using Microsoft.AspNetCore.Http;

namespace AuthService.Infrastructure.Common
{
    public static class ClientIpHelper
    {
        public static string GetClientIp(HttpContext context)
        {
            // 1. X-Forwarded-For kontrolü
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var ip = forwardedFor.ToString().Split(',').FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(ip))
                    return ip;
            }

            // 2. X-Real-IP kontrolü
            if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
            {
                if (!string.IsNullOrWhiteSpace(realIp))
                    return realIp!;
            }

            // 3. RemoteIpAddress fallback
            return context.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";
        }

    }
}
