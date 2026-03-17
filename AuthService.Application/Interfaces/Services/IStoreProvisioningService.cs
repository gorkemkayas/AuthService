namespace AuthService.Application.Interfaces.Services
{
    public interface IStoreProvisioningService
    {
        Task ProvisionStoreAsync(int tenantId,string slug);
    }
}
