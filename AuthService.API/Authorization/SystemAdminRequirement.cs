using Microsoft.AspNetCore.Authorization;

namespace AuthService.API.Authorization;

public class SystemAdminRequirement : IAuthorizationRequirement
{
    // Bu requirement token_type = "system" ve role = "SuperAdmin" kontrolü yapar
}
