using System.Linq.Expressions;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomersService.Persistence.Repositories
{
    public abstract class BaseRepository<T>(CustomerServiceDbContext context) : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task CreateAsync(T entity)
        {
            _dbSet.Add(entity);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(int skip, int take) => await _dbSet.Skip(skip).Take(take).ToListAsync();
        
        public async Task<T?> GetByConditionAsync(Expression<Func<T, bool>> condition) =>
            await _dbSet.Where(condition).SingleOrDefaultAsync();

        public async Task<List<T>> GetAllByConditionAsync(Expression<Func<T, bool>> condition) =>
          await _dbSet.Where(condition).ToListAsync();
    }
}
