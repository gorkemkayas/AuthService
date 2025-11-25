namespace AuthService.Infrastructure.Mapping
{
    public interface IEntityMapper
    {
        TData MapToData<TDomain, TData>(TDomain domainEntity) where TDomain : class where TData : class;
        TDomain MapToDomain<TDomain, TData>(TData dataEntity) where TDomain : class where TData : class;
    }

}
