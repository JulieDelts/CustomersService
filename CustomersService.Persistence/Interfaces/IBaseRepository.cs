
using System.Linq.Expressions;

namespace CustomersService.Persistence.Interfaces
{
    public interface IBaseRepository<T> 
    {
        Task CreateAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task DeleteAsync(T entity);
        Task<T?> GetByConditionAsync(Expression<Func<T, bool>> condition);
    }
}
