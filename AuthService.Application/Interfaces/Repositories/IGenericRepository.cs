using AuthService.Application.Interfaces.Specifications;
using System.Linq.Expressions;

namespace AuthService.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(ISpecification<T>? specification, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    }
}
