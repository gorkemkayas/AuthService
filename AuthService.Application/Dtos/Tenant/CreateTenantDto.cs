using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Dtos.Tenant
{
    public class CreateTenantDto
    {
        public string Name { get; set; } = null!;  // Mağaza/firma adı
        public string Domain { get; set; } = null!; // Opsiyonel, mağaza alan adı
    }
}
