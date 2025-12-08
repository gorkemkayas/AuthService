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
            var clientType = context.Request.Headers["X-Client-Type"].ToString();

            if (string.IsNullOrWhiteSpace(clientType))
                clientType = "web"; // default

            context.Items["X-Client-Type"] = clientType;

            await _next(context);
        }
    }
}
