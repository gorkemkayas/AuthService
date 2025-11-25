namespace AuthService.Infrastructure.Mapping
{
    public static class TenantMapper
    {
        // Domain → Data
        public static AuthService.Infrastructure.Persistance.Entities.Tenant ToData(AuthService.Domain.Entities.Tenant domainTenant)
        {
            if (domainTenant == null) throw new ArgumentNullException(nameof(domainTenant));

            return new AuthService.Infrastructure.Persistance.Entities.Tenant
            {
                Id = domainTenant.Id,
                Name = domainTenant.Name,
                IsActive = domainTenant.IsActive,
                IsSystem = domainTenant.IsSystem,
                CreatedAt = domainTenant.CreatedAt,
                UpdatedAt = domainTenant.UpdatedAt,
                IsDeleted = domainTenant.IsDeleted
            };
        }

        // Data → Domain
        public static AuthService.Domain.Entities.Tenant ToDomain(AuthService.Infrastructure.Persistance.Entities.Tenant dataTenant)
        {
            if (dataTenant == null) throw new ArgumentNullException(nameof(dataTenant));

            var domainTenant = new AuthService.Domain.Entities.Tenant
            {
                Id = dataTenant.Id,
                Name = dataTenant.Name,
                IsActive = dataTenant.IsActive,
                IsSystem = dataTenant.IsSystem,
                CreatedAt = dataTenant.CreatedAt,
                UpdatedAt = dataTenant.UpdatedAt,
                IsDeleted = dataTenant.IsDeleted
            };

            // Navigation property mapping
            if (dataTenant.ApplicationUsers != null && dataTenant.ApplicationUsers.Any())
            {
                domainTenant.Users = dataTenant.ApplicationUsers
                    .Select(UserMapper.ToDomain)
                    .ToList();
            }

            return domainTenant;
        }
    }


}
