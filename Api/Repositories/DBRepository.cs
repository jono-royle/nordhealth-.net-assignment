
using Api.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace Api.Repositories
{
    public class DBRepository<T> : IDBRepository<T> where T : class
    {
        private readonly VetAppDbContext _db;
        private readonly DbSet<T> _dbSet;

        public DBRepository(VetAppDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity;
        }

        public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        public async Task<List<T>> ScanAsync(
            Func<IQueryable<T>, IQueryable<T>> query,
            CancellationToken ct = default)
        {
            var request = query(_db.Set<T>());
            return await request.ToListAsync(ct);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
