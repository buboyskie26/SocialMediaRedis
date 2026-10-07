using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Social_Media.Repository.Base
{
    public class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            // Apply includes (eager loading)
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            // Return single entity or null
            return await query.FirstOrDefaultAsync(predicate);
        }
    }
}
