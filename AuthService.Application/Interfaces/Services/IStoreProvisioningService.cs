using AuthService.Application.Results;

namespace AuthService.Application.Interfaces.Services
{
    public interface IStoreProvisioningService
    {
        Task<ServiceResult> ProvisionStoreAsync(int tenantId, string slug);
        Task<ServiceResult> ProvisionCustomerAsync(int tenantId, Guid externalUserId, string email, string firstName, string lastName);
    }
}
