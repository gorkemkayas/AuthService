using AuthService.Application.Common;
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

        private HttpContext? HttpContext => _httpContextAccessor.HttpContext;
        public string ClientType
        {
            get
            {
                if (HttpContext?.Items.TryGetValue(ClientContextKeys.ClientType, out var value) == true)
                    return value?.ToString() ?? ClientTypes.Web;

                return ClientTypes.Web;
            }
        }

        public string? DeviceId
        {
            get
            {
                if (HttpContext?.Items.TryGetValue(ClientContextKeys.DeviceId, out var value) == true)
                    return value?.ToString();

                return null;
            }
        }
    }
}
