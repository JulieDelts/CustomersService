using System.Linq.Expressions;
using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task CreateAsync(T entity);
    Task<T?> GetByConditionAsync(Expression<Func<T, bool>> condition);
    Task<List<T>> GetAllByConditionAsync(Expression<Func<T, bool>> condition);
    Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize);
}
