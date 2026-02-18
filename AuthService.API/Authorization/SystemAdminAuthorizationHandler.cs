using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthService.API.Authorization;

public class SystemAdminAuthorizationHandler : AuthorizationHandler<SystemAdminRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SystemAdminRequirement requirement)
    {
        // Token'daki "token_type" claim'ini kontrol et
        var tokenTypeClaim = context.User.FindFirst("token_type");
        if (tokenTypeClaim == null || tokenTypeClaim.Value != "system")
        {
            return Task.CompletedTask; // Başarısız - system token değil
        }

        // "SuperAdmin" rolünü kontrol et
        if (!context.User.IsInRole("SuperAdmin"))
        {
            return Task.CompletedTask; // Başarısız - SuperAdmin rolü yok
        }

        // Her iki koşul da sağlandı
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
