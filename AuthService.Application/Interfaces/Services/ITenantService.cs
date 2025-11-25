using AuthService.Application.Results;

namespace AuthService.Application.Interfaces.Services
{
    public interface ITenantService
    {
        Task<ServiceResult> IsTenantExistsAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<ServiceResult<Tenant>> GetTenantByIdAsync(int tenantId, CancellationToken cancellationToken = default);


    }

}
