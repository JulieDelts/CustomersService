
namespace CustomersService.Persistence.Interfaces
{
    public interface ICustomerUnitOfWork
    {
        void Begin();
        void Commit();
        void Rollback();
    }
}
