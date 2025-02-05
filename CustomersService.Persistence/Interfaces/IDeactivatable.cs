using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces
{
    public interface IDeactivatable<T> where T : BaseEntity
    {
        Task ActivateAsync(T entity);
        Task DeactivateAsync(T entity);
    }
}
