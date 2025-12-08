using AuthService.Application.Interfaces.Contexts;

namespace AuthService.API.Contexts
{
    public class ClientContext : IClientContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClientContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string ClientType
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                if (context != null && context.Items.ContainsKey("X-Client-Type"))
                {
                    return context.Items["X-Client-Type"]?.ToString() ?? "web";
                }
                return "web"; // default
            }
        }
    }
}
