using System.Linq.Expressions;

namespace Social_Media.Repository.Base
{
    public interface IBaseRepository<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
params Expression<Func<TEntity, object>>[] includes);

        // Returns a single entity or null
        Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includes);
    }
}
