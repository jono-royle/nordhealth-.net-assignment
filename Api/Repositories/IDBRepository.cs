using System.Linq.Expressions;

namespace Api.Repositories
{
    public interface IDBRepository<T> where T : class
    {
        IQueryable<T> QueryAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
    }
}
