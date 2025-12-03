using AuthService.Application.Interfaces.Repositories;
using AuthService.Infrastructure.Mapping;
using AuthService.Infrastructure.Persistance.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistance.Repositories
{
    public class GenericRepository<TDomain, TData, TKey> : IGenericRepository<TDomain,TKey> where TDomain : class where TData : class
    {
        protected readonly AuthDbContext _context;
        private readonly DbSet<TData> _dbSet;
        protected readonly IEntityMapper _mapper;

        public GenericRepository(AuthDbContext context, IEntityMapper mapper)
        {
            _context = context;
            _dbSet = _context.Set<TData>();
            _mapper = mapper;
        }

        public async Task AddAsync(TDomain entity, CancellationToken cancellationToken = default)
        {
            var dataEntity = _mapper.MapToData<TDomain, TData>(entity);
            await _dbSet.AddAsync(dataEntity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<TDomain> entities, CancellationToken cancellationToken = default)
        {
            var dataEntities = entities.Select(e => _mapper.MapToData<TDomain, TData>(e));
            await _dbSet.AddRangeAsync(dataEntities, cancellationToken);
        }

        public Task<bool> ExistsAsync(TKey? id, bool? onlyActive = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();
            if (id != null)
            {
                query = query.Where(e => EF.Property<TKey>(e, "Id").Equals(id));
            }
            if (onlyActive.HasValue && onlyActive.Value)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsActive") == true);
            }
            return query.AnyAsync(cancellationToken);
        }

        public async Task<TDomain?> FindAsync(TKey id, bool? onlyActive = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();

            query = query.Where(e => EF.Property<TKey>(e, "Id").Equals(id));

            if (onlyActive.HasValue && onlyActive.Value)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsActive") == true);
            }

            var dataEntity = await query.FirstOrDefaultAsync(cancellationToken);
            return dataEntity == null ? null : _mapper.MapToDomain<TDomain, TData>(dataEntity);
        }


        public async Task<IEnumerable<TDomain>?> GetAllAsync(TKey? id, bool? onlyActive = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();
            if (id != null)
            {
                query = query.Where(e => EF.Property<TKey>(e, "Id").Equals(id));
            }
            if (onlyActive.HasValue && onlyActive.Value)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsActive") == true);
            }
            var dataEntities = await query.ToListAsync(cancellationToken);
            var datas = dataEntities.Select(e => _mapper.MapToDomain<TDomain, TData>(e));
            return datas;

        }
        public async Task<IEnumerable<TDomain>?> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities =  await _dbSet.AsNoTracking().ToListAsync();
            return entities.Select(e => _mapper.MapToDomain<TDomain, TData>(e));
        }

        public async Task<TDomain?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(new[] { id }, cancellationToken);
            if (entity == null)
                return null;
            return _mapper.MapToDomain<TDomain, TData>(entity);
        }

        public void Update(TDomain entity)
        {
            var mappedEntity = _mapper.MapToData<TDomain, TData>(entity);
            var trackedEntity = _context.ChangeTracker.Entries<TData>().FirstOrDefault(e => e.Entity.Equals(mappedEntity));
            if (trackedEntity != null)
            {
                trackedEntity.CurrentValues.SetValues(mappedEntity);
            }

            _dbSet.Update(mappedEntity);

        }

        public async Task<TDomain?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.Where(e => EF.Property<string>(e, "Name") == name).FirstOrDefaultAsync(cancellationToken);
            return entity == null ? null : _mapper.MapToDomain<TDomain, TData>(entity);
        }

    }

}
