namespace AuthService.Application.Dtos.Tenant
{
    public sealed class TenantRegistrationResponse
    {
        public int TenantId { get; set; }
        public string? StoreId { get; set; }
        public string? StoreSlug { get; set; }
        public bool RequiresEmailVerification { get; set; }
        public string Message { get; set; } = null!;
    }
}