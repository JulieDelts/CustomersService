using System.Linq.Expressions;
using CustomersService.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomersService.Persistence.Repositories
{
    public abstract class BaseRepository<T>(CustomerServiceDbContext context) : IBaseRepository<T> where T : class
    {
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task CreateAsync(T entity)
        {
            _dbSet.Add(entity);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        
        public async Task<T?> GetByConditionAsync(Expression<Func<T, bool>> condition) =>
            await _dbSet.Where(condition).SingleOrDefaultAsync();

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
