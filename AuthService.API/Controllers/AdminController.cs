using Asp.Versioning;
using AuthService.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/[controller]")]
    public class AdminController : BaseController
    {
        public AdminController(IOptions<TokenOptions> tokenOptions) : base(tokenOptions)
        {
            
        }
    }
}
