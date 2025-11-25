using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistance.Entities;

namespace AuthService.Infrastructure.Mapping
{
    public class EntityMapper : IEntityMapper
    {
        public TData MapToData<TDomain, TData>(TDomain domainEntity)
            where TDomain : class
            where TData : class
        {
            return domainEntity switch
            {
                User user => UserMapper.ToData(user) as TData
                              ?? throw new InvalidOperationException("Mapping failed"),
                AuthService.Domain.Entities.RefreshToken token => RefreshTokenMapper.ToData(token) as TData
                                     ?? throw new InvalidOperationException("Mapping failed"),
                AuthService.Domain.Entities.Tenant tenant => TenantMapper.ToData(tenant) as TData
                                  ?? throw new InvalidOperationException("Mapping failed"),
                _ => throw new InvalidOperationException("Mapping for this type is not implemented")
            };
        }
        public TDomain MapToDomain<TDomain, TData>(TData dataEntity)
            where TDomain : class
            where TData : class
        {
            return dataEntity switch
            {
                ApplicationUser user => UserMapper.ToDomain(user) as TDomain
                                       ?? throw new InvalidOperationException("Mapping failed"),
                AuthService.Infrastructure.Persistance.Entities.RefreshToken token =>
                    RefreshTokenMapper.ToDomain(token) as TDomain
                    ?? throw new InvalidOperationException("Mapping failed"),
                AuthService.Infrastructure.Persistance.Entities.Tenant tenant =>
                    TenantMapper.ToDomain(tenant) as TDomain
                    ?? throw new InvalidOperationException("Mapping failed"),
                _ => throw new InvalidOperationException("Mapping for this type is not implemented")
            };
        }
    }

}
