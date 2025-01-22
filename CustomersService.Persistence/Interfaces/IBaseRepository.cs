
using System.Linq.Expressions;

namespace CustomersService.Persistence.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task CreateAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task ActivateAsync(T entity);
        Task DeactivateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T?> GetByConditionAsync(Expression<Func<T, bool>> condition);
    }
}
