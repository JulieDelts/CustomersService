using System.Linq.Expressions;
using CustomersService.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomersService.Persistence.Repositories
{
    public abstract class BaseRepository<T>(CustomerServiceDbContext context) : IBaseRepository<T> where T : class
    {
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public virtual async Task CreateAsync(T entity)
        {
            DbSet.Add(entity);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await DbSet.ToListAsync();
        
        public virtual async Task<T?> GetByConditionAsync(Expression<Func<T, bool>> condition) =>
            await DbSet.Where(condition).SingleOrDefaultAsync();

        public virtual async Task DeleteAsync(T entity)
        {
            DbSet.Remove(entity);
        }

        public abstract Task ActivateAsync(T entity);

        public abstract Task DeactivateAsync(T entity);
    }
}
