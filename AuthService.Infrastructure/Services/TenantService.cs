using AuthService.Application.Dtos.Integration;
using AuthService.Application.Common;
using AuthService.Application.Dtos.Tenant;
using AuthService.Application.Dtos.User;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Results;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Common;
using AuthService.Infrastructure.Persistance.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AuthDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly IStoreProvisioningService _storeProvisioningService;
    private readonly ILogger<TenantService> _logger;

    public TenantService(
        IUnitOfWork unitOfWork,
        AuthDbContext dbContext,
        IUserService userService,
        IStoreProvisioningService storeProvisioningService,
        ILogger<TenantService> logger)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _userService = userService;
        _storeProvisioningService = storeProvisioningService;
        _logger = logger;
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
        var tenantName = createTenantDto.Name.Trim();
        var existingTenant = await _unitOfWork.Tenants.GetByNameAsync(tenantName);
        if (existingTenant is not null)
            return ServiceResult<TenantDto>.Fail("Tenant with the same name already exists.");

        var existingDomains = _unitOfWork.Tenants.GetAllDomainAddresses();
        var newTenant = new Tenant
        {
            Name = tenantName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Email = GenerateTenantEmailFromName(tenantName), // Domain yerine Name kullan
        };
        await _unitOfWork.Tenants.AddAsync(newTenant);
        await _unitOfWork.SaveChangesAsync();
        var persistedTenant = await _unitOfWork.Tenants.GetByNameAsync(newTenant.Name);
        var tenantDto = new TenantDto
        {
            Id = persistedTenant!.Id,
            Name = newTenant.Name,
            IsActive = newTenant.IsActive,
            CreatedAt = newTenant.CreatedAt,
            UpdatedAt = newTenant.UpdatedAt
        };
        return ServiceResult<TenantDto>.Ok(tenantDto, "Tenant created successfully.");
    }

    public async Task<ServiceResult<TenantRegistrationResponse>> RegisterTenantAsync(RegisterTenantRequest request)
    {
        var normalizedPlanCode = NormalizePlanCode(request.PlanCode);
        if (!IsSupportedPlanCode(normalizedPlanCode))
            return ServiceResult<TenantRegistrationResponse>.Fail("PlanCode must be one of: starter, growth, premium.", ErrorCodes.ValidationError);

        var existingTenant = await _unitOfWork.Tenants.GetByNameAsync(request.Name);
        if (existingTenant is not null)
            return ServiceResult<TenantRegistrationResponse>.Fail("Tenant with the same name already exists.", ErrorCodes.AlreadyExists);

        var tenantName = request.Name.Trim();
        var tenant = new Tenant
        {
            Name = tenantName,
            IsActive = true,
            IsSystem = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Email = GenerateTenantEmailFromName(tenantName),
            HashedPassword = "0000000000"
        };

        await _unitOfWork.Tenants.AddAsync(tenant);
        await _unitOfWork.SaveChangesAsync();

        var persistedTenant = await _unitOfWork.Tenants.GetByNameAsync(tenantName);
        if (persistedTenant is null)
        {
            return ServiceResult<TenantRegistrationResponse>.Fail("Tenant creation failed.", ErrorCodes.Unexpected);
        }

        var ownerResult = await _userService.CreateTenantUserAsync(new CreateTenantUserRequest
        {
            Name = request.Owner.Name.Trim(),
            Surname = request.Owner.Surname.Trim(),
            Email = request.Owner.Email.Trim(),
            Password = request.Owner.Password,
            TenantId = persistedTenant.Id
        });

        if (!ownerResult.Success)
        {
            await MarkTenantRegistrationFailedAsync(persistedTenant.Id);
            return ServiceResult<TenantRegistrationResponse>.Fail(ownerResult.Message ?? "Failed to create tenant owner.", ownerResult.ErrorCode ?? ErrorCodes.Unexpected);
        }

        var roleResult = await _userService.AssignRolesToUserAsync(
            ownerResult.Data!.TenantUserId.ToString(),
            persistedTenant.Id,
            new List<string> { "TenantAdmin" });

        if (!roleResult.Success)
        {
            await MarkTenantRegistrationFailedAsync(persistedTenant.Id);
            return ServiceResult<TenantRegistrationResponse>.Fail(
                roleResult.Message ?? "Failed to assign TenantAdmin role to tenant owner.",
                roleResult.ErrorCode ?? ErrorCodes.Unexpected);
        }

        var provisioningResult = await _storeProvisioningService.ProvisionStoreAsync(persistedTenant.Id, persistedTenant.Name, normalizedPlanCode);
        string? storeId = null;
        string? storeSlug = null;

        if (provisioningResult.Success)
        {
            storeId = provisioningResult.Data?.StoreId;
            storeSlug = provisioningResult.Data?.StoreSlug;
        }
        else
        {
            _logger.LogError("Store provisioning failed for tenant {TenantId}: {Message}", persistedTenant.Id, provisioningResult.Message);
        }

        var response = new TenantRegistrationResponse
        {
            TenantId = persistedTenant.Id,
            StoreId = storeId,
            StoreSlug = storeSlug,
            RequiresEmailVerification = false,
            Message = "Store owner registration completed."
        };

        return ServiceResult<TenantRegistrationResponse>.Ok(response, response.Message);
    }

    private async Task MarkTenantRegistrationFailedAsync(int tenantId)
    {
        var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(item => item.Id == tenantId);
        if (tenant is null)
            return;

        tenant.IsActive = false;
        tenant.IsDeleted = true;
        tenant.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<ServiceResult> DeleteTenantAsync(int tenantId)
    {
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId);
        if (tenant is null)
            return ServiceResult.Fail("Tenant does not exist.");
        if (tenant.IsDeleted)
            return ServiceResult.Fail("Tenant is already inactive.");
        tenant.IsDeleted = true;
        tenant.IsActive = false;
        tenant.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Tenants.Update(tenant); // entity'i modified olarak işaretledik.
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
        tenant.IsDeleted = false;
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
    public static string GenerateTenantEmailFromDomain(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain))
            throw new ArgumentException("Domain cannot be empty", nameof(domain));

        var parts = domain.Split('.', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            throw new ArgumentException("Domain must be in format subdomain.domain.tld (e.g. cengiztech.kayas.dev)");

        var subdomain = parts[0];
        var baseDomain = string.Join('.', parts.Skip(1));

        return $"{subdomain}@{baseDomain}";
    }
    public static string GenerateTenantEmailFromName(string tenantName)
    {
        if (string.IsNullOrWhiteSpace(tenantName))
            throw new ArgumentException("Tenant name cannot be empty", nameof(tenantName));

        // Tenant name'i email-friendly formata çevir:
        // - Küçük harfe çevir
        // - Boşlukları ve özel karakterleri kaldır/normalize et
        var emailPrefix = tenantName
            .ToLowerInvariant()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("_", "");

        // Sadece alfanumerik karakterleri tut
        emailPrefix = new string(emailPrefix.Where(char.IsLetterOrDigit).ToArray());

        if (string.IsNullOrWhiteSpace(emailPrefix))
            throw new ArgumentException("Tenant name must contain at least one alphanumeric character", nameof(tenantName));

        return $"{emailPrefix}@kayas.dev";
    }

    private static string NormalizePlanCode(string? planCode)
    {
        if (string.IsNullOrWhiteSpace(planCode))
            return "starter";

        return planCode.Trim().ToLowerInvariant();
    }

    private static bool IsSupportedPlanCode(string planCode)
    {
        return planCode is "starter" or "growth" or "premium";
    }

}
