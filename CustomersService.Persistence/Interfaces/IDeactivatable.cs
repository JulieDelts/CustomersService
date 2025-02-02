
namespace CustomersService.Persistence.Interfaces
{
    public interface IDeactivatable<T>
    {
        Task ActivateAsync(T entity);
        Task DeactivateAsync(T entity);
    }
}
