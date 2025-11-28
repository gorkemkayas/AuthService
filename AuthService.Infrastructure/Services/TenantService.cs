using AuthService.Application.Dtos.Tenant;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Results;
using AuthService.Infrastructure.Common;

namespace AuthService.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IUnitOfWork _unitOfWork;
    public TenantService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ServiceResult<TenantDto>> GetTenantByIdAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult<TenantDto>.Fail("Tenant does not exist.");
        var tenantDto = new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt
        };
        return ServiceResult<TenantDto>.Ok(tenantDto);
    }
    public async Task<ServiceResult<TenantDto>> GetTenantByNameAsync(string tenantName)
    {
        var tenant = await _unitOfWork.Tenants.GetByNameAsync(tenantName);
        if (tenant is null)
            return ServiceResult<TenantDto>.Fail("Tenant does not exist.");
        var tenantDto = new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt
        };
        return ServiceResult<TenantDto>.Ok(tenantDto);
    }
    public async Task<ServiceResult<TenantDto>> CreateNewTenantAsync(CreateTenantDto createTenantDto)
    {
        var existingTenant = await _unitOfWork.Tenants.GetByNameAsync(createTenantDto.Name);
        if (existingTenant is not null)
            return ServiceResult<TenantDto>.Fail("Tenant with the same name already exists.");

        var existingDomains = _unitOfWork.Tenants.GetAllDomainAddresses();
        var newTenant = new Domain.Entities.Tenant
        {
            Name = createTenantDto.Name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Email = GeneratorHelper.GenerateFirmMail(createTenantDto.Name),
            Domain = GeneratorHelper.IsDomainAvailable(createTenantDto.Domain, existingDomains) ? GeneratorHelper.GetDomainAddress(createTenantDto.Domain) : GeneratorHelper.GetDomainAddress(GeneratorHelper.GenerateDomain(createTenantDto.Domain, existingDomains))
        };
        await _unitOfWork.Tenants.AddAsync(newTenant);
        await _unitOfWork.SaveChangesAsync();
        var tenantDto = new TenantDto
        {
            Id = newTenant.Id,
            Name = newTenant.Name,
            IsActive = newTenant.IsActive,
            CreatedAt = newTenant.CreatedAt,
            UpdatedAt = newTenant.UpdatedAt
        };
        return ServiceResult<TenantDto>.Ok(tenantDto, "Tenant created successfully.");
    }
    public async Task<ServiceResult> DisableTenantAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult.Fail("Tenant does not exist.");
        if (!tenant.IsActive)
            return ServiceResult.Fail("Tenant is already inactive.");
        tenant.IsActive = false;
        tenant.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Tenants.Update(tenant);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Ok("Tenant disabled successfully.");
    }
    public async Task<ServiceResult> EnableTenantAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult.Fail("Tenant does not exist.");
        if (tenant.IsActive)
            return ServiceResult.Fail("Tenant is already active.");
        tenant.IsActive = true;
        tenant.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Tenants.Update(tenant);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Ok("Tenant enabled successfully.");
    }
    public async Task<ServiceResult<List<TenantDto>>> GetAllTenantsAsync()
    {
        var tenants = await _unitOfWork.Tenants.GetAllAsync();
        if(tenants is null || !tenants.Any())
            return ServiceResult<List<TenantDto>>.Fail("No tenants found.");
        var tenantDtos = tenants.Select(tenant => new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt
        }).ToList();
        return ServiceResult<List<TenantDto>>.Ok(tenantDtos);
    }
    public async Task<ServiceResult> IsTenantExistsAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        return tenant is not null
            ? ServiceResult.Ok()
            : ServiceResult.Fail("Tenant does not exist.");
    }
    public async Task<ServiceResult> IsTenantEmptyAsync(int tenantId)
    {
        var userCount = await _unitOfWork.Users.GetCountByTenantIdAsync(tenantId);
        return userCount == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail("Tenant is not empty.");
    }
    public async Task<ServiceResult> IsTenantActiveAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        return tenant is not null && tenant.IsActive
            ? ServiceResult.Ok()
            : ServiceResult.Fail("Tenant is not active.");
    }
    public async Task<ServiceResult> IsTenantInactiveAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        return tenant is not null && !tenant.IsActive
            ? ServiceResult.Ok()
            : ServiceResult.Fail("Tenant is not inactive.");
    }
    public async Task<ServiceResult> IsTenantDeletableAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult.Fail("Tenant does not exist.");
        var userCount = await _unitOfWork.Users.GetCountByTenantIdAsync(tenantId);
        return userCount == 0
            ? ServiceResult.Ok()
            : ServiceResult.Fail("Tenant is not deletable because it has associated users.");
    }
    public async Task<ServiceResult> IsTenantCreatableAsync(string tenantName)
    {
        var existingTenant = await _unitOfWork.Tenants.GetByNameAsync(tenantName);
        return existingTenant is null
            ? ServiceResult.Ok()
            : ServiceResult.Fail("Tenant with the same name already exists.");
    }
    public async Task<ServiceResult<int>> GetTenantUserCountAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult<int>.Fail("Tenant does not exist.");
        var userCount = await _unitOfWork.Users.GetCountByTenantIdAsync(tenantId);
        return ServiceResult<int>.Ok(userCount);
    }
    public async Task<ServiceResult<bool>> IsTenantNameUniqueAsync(string tenantName)
    {
        var existingTenant = await _unitOfWork.Tenants.GetByNameAsync(tenantName);
        bool isUnique = existingTenant is null;
        return ServiceResult<bool>.Ok(isUnique);
    }
    public async Task<ServiceResult<int>> GetActiveTenantCountAsync()
    {
        var activeTenantCount = await _unitOfWork.Tenants.GetActiveTenantCountAsync();
        return ServiceResult<int>.Ok(activeTenantCount);
    }
    public async Task<ServiceResult<int>> GetInactiveTenantCountAsync()
    {
        var inactiveTenantCount = await _unitOfWork.Tenants.GetInactiveTenantCountAsync();
        return ServiceResult<int>.Ok(inactiveTenantCount);
    }
    public async Task<ServiceResult<int>> GetTotalTenantCountAsync()
    {
        var totalTenantCount = await _unitOfWork.Tenants.GetTotalTenantCountAsync();
        return ServiceResult<int>.Ok(totalTenantCount);
    }
    public async Task<ServiceResult<bool>> DoesTenantHaveUsersAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult<bool>.Fail("Tenant does not exist.");
        var userCount = await _unitOfWork.Users.GetCountByTenantIdAsync(tenantId);
        bool hasUsers = userCount > 0;
        return ServiceResult<bool>.Ok(hasUsers);
    }
    public async Task<ServiceResult<bool>> DoesTenantHaveNoUsersAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult<bool>.Fail("Tenant does not exist.");
        var userCount = await _unitOfWork.Users.GetCountByTenantIdAsync(tenantId);
        bool hasNoUsers = userCount == 0;
        return ServiceResult<bool>.Ok(hasNoUsers);
    }
    public async Task<ServiceResult<bool>> IsTenantActiveAndHasUsersAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult<bool>.Fail("Tenant does not exist.");
        if (!tenant.IsActive)
            return ServiceResult<bool>.Ok(false);
        var userCount = await _unitOfWork.Users.GetCountByTenantIdAsync(tenantId);
        bool isActiveAndHasUsers = userCount > 0;
        return ServiceResult<bool>.Ok(isActiveAndHasUsers);
    }
    public async Task<ServiceResult<bool>> IsTenantInactiveAndHasNoUsersAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult<bool>.Fail("Tenant does not exist.");
        if (tenant.IsActive)
            return ServiceResult<bool>.Ok(false);
        var userCount = await _unitOfWork.Users.GetCountByTenantIdAsync(tenantId);
        bool isInactiveAndHasNoUsers = userCount == 0;
        return ServiceResult<bool>.Ok(isInactiveAndHasNoUsers);
    }
    public async Task<ServiceResult<bool>> CanTenantBeActivatedAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult<bool>.Fail("Tenant does not exist.");
        bool canBeActivated = !tenant.IsActive;
        return ServiceResult<bool>.Ok(canBeActivated);
    }
    public async Task<ServiceResult<bool>> CanTenantBeDeactivatedAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult<bool>.Fail("Tenant does not exist.");
        bool canBeDeactivated = tenant.IsActive;
        return ServiceResult<bool>.Ok(canBeDeactivated);
    }
    public async Task<ServiceResult<bool>> CanTenantBeRenamedAsync(int tenantId, string newTenantName)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult<bool>.Fail("Tenant does not exist.");
        var existingTenant = await _unitOfWork.Tenants.GetByNameAsync(newTenantName);
        bool canBeRenamed = existingTenant is null || existingTenant.Id == tenantId;
        return ServiceResult<bool>.Ok(canBeRenamed);
    }
}
