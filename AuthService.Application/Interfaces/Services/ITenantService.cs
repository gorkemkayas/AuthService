using System.Collections.Generic;
using System.Threading.Tasks;
using AuthService.Application.Dtos.Tenant;
using AuthService.Application.Results;

namespace AuthService.Application.Interfaces.Services
{
    public interface ITenantService
    {
        Task<ServiceResult<TenantDto>> GetTenantByIdAsync(int tenantId);
        Task<ServiceResult<TenantDto>> GetTenantByNameAsync(string tenantName);
        Task<ServiceResult<TenantDto>> CreateNewTenantAsync(CreateTenantDto createTenantDto);
        Task<ServiceResult> DeleteTenantAsync(int tenantId);
        Task<ServiceResult> EnableTenantAsync(int tenantId);
        Task<ServiceResult<List<TenantDto>>> GetAllTenantsAsync();
        Task<ServiceResult> IsTenantExistsAsync(int tenantId);
        Task<ServiceResult> IsTenantEmptyAsync(int tenantId);
        Task<ServiceResult> IsTenantActiveAsync(int tenantId);
        Task<ServiceResult> IsTenantInactiveAsync(int tenantId);
        Task<ServiceResult> IsTenantDeletableAsync(int tenantId);
        Task<ServiceResult> IsTenantCreatableAsync(string tenantName);
        Task<ServiceResult<int>> GetTenantUserCountAsync(int tenantId);
        Task<ServiceResult<bool>> IsTenantNameUniqueAsync(string tenantName);
        Task<ServiceResult<int>> GetActiveTenantCountAsync();
        Task<ServiceResult<int>> GetInactiveTenantCountAsync();
        Task<ServiceResult<int>> GetTotalTenantCountAsync();
        Task<ServiceResult<bool>> DoesTenantHaveUsersAsync(int tenantId);
        Task<ServiceResult<bool>> DoesTenantHaveNoUsersAsync(int tenantId);
        Task<ServiceResult<bool>> IsTenantActiveAndHasUsersAsync(int tenantId);
        Task<ServiceResult<bool>> IsTenantInactiveAndHasNoUsersAsync(int tenantId);
        Task<ServiceResult<bool>> CanTenantBeActivatedAsync(int tenantId);
        Task<ServiceResult<bool>> CanTenantBeDeactivatedAsync(int tenantId);
        Task<ServiceResult<bool>> CanTenantBeRenamedAsync(int tenantId, string newTenantName);
    }
}
