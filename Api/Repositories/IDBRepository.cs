using System.Linq.Expressions;

namespace Api.Repositories
{
    public interface IDBRepository<T> where T : class
    {
        Task<List<T>> ScanAsync(Func<IQueryable<T>, IQueryable<T>> query, CancellationToken ct = default);
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
    }
}
