namespace AuthService.Application.Dtos.Tenant
{
    public class UpdateTenantDto
    {
        public string? Name { get; set; }
        public string? Domain { get; set; }
        public bool? IsActive { get; set; }
    }
}
