namespace AuthService.Application.Interfaces.Repositories
{
    public interface IGenericRepository<TDomain,TKey> where TDomain : class
    {
        Task<TDomain?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        Task<TDomain?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        // Domain-friendly filtre parametreleri ile listeleme
        Task<IEnumerable<TDomain>?> GetAllAsync(TKey? id, bool? onlyActive = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<TDomain>?> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TDomain?> FindAsync(TKey id, bool? onlyActive = null, CancellationToken cancellationToken = default);
        Task AddAsync(TDomain entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TDomain> entities, CancellationToken cancellationToken = default);
        void Update(TDomain entity);
        Task<bool> ExistsAsync(TKey? id, bool? onlyActive = null, CancellationToken cancellationToken = default);
    }

}
