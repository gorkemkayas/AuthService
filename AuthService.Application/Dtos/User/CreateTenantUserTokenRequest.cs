using AuthService.Application.Common;

namespace AuthService.Application.Dtos.User
{
    public interface CreateTokenRequest
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int TenantId { get; set; }
        // public string TenantDomain { get; set; }
        // tracking için eklendi.
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string DeviceName { get; set; }
        public string ClientType { get; set; }
        public string? DeviceId { get; set; }
    }
    public class CreateTenantUserTokenRequest : CreateTokenRequest
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int TenantId { get; set; }
        // public string TenantDomain { get; set; } = null!;

        // tracking için eklendi.
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
        public string DeviceName { get; set; } = null!;

        public string ClientType { get; set; } = null!;
        public string? DeviceId { get; set; }
    }
    public class CreateAdminUserTokenRequest : CreateTokenRequest
    {
        public CreateAdminUserTokenRequest()
        {
            TenantId = SystemConstants.SystemTenantId;
            TenantDomain = SystemConstants.SystemTenantDomain;
        }
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int TenantId { get; set; }
        public string TenantDomain { get; set; }

        // tracking için eklendi.
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
        public string DeviceName { get; set; } = null!;

        public string ClientType { get; set; } = null!;
        public string? DeviceId { get; set; }
    }
}
