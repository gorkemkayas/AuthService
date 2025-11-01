using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Specifications;
using AuthService.Infrastructure.Persistance.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AuthDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AuthDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
            => await _dbSet.AddAsync(entity, cancellationToken);
        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            => await _dbSet.AddRangeAsync(entities, cancellationToken);
        public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification);
            return await query.CountAsync(cancellationToken);
        }
        public async Task<bool> ExistsAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification);
            return await query.AnyAsync(cancellationToken);
        }
        public async Task<IEnumerable<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification);
            return await query.ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<T>> GetAllAsync(ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        {
            var query = specification != null ? ApplySpecification(specification) : _dbSet.AsQueryable();
            return await query.ToListAsync(cancellationToken);
        }
        public Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
            => _dbSet.FindAsync(new[] { id }, cancellationToken).AsTask();
        public void Remove(T entity) => _dbSet.Remove(entity);
        public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
        public void Update(T entity) => _dbSet.Update(entity);
        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            var query = _dbSet.AsQueryable();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            foreach (var include in specification.Includes)
                query = query.Include(include);

            if (specification.OrderBy != null)
                query = query.OrderBy(specification.OrderBy);

            if (specification.OrderByDescending != null)
                query = query.OrderByDescending(specification.OrderByDescending);

            if (specification.Skip.HasValue)
                query = query.Skip(specification.Skip.Value);

            if (specification.Take.HasValue)
                query = query.Take(specification.Take.Value);

            return query;
        }
    }

}
