namespace AuthService.Infrastructure.Mapping
{
    public interface IEntityMapper
    {
        TData MapToData<TDomain, TData>(TDomain domainEntity) where TDomain : class where TData : class;
        IEnumerable<TData> MapToData<TDomain, TData>(IEnumerable<TDomain> domainEntities) where TDomain : class where TData : class;
        TDomain MapToDomain<TDomain, TData>(TData dataEntity) where TDomain : class where TData : class;
        IEnumerable<TDomain> MapToDomain<TDomain, TData>(IEnumerable<TData> dataEntities) where TDomain : class where TData : class ;
    }

}
