using AuthService.Application.Results;
using AuthService.Application.Dtos.Integration;

namespace AuthService.Application.Interfaces.Services
{
    public interface IStoreProvisioningService
    {
        Task<ServiceResult<StoreProvisioningResponse>> ProvisionStoreAsync(int tenantId, string name);
        Task<ServiceResult<StoreProvisioningResponse>> ProvisionStoreAsync(int tenantId, string name, string planCode);
        Task<ServiceResult> ProvisionCustomerAsync(int tenantId, Guid externalUserId, string email, string firstName, string lastName);
    }
}
