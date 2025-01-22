
namespace CustomersService.Persistence.Interfaces
{
    public interface IUnitOfWork: IDisposable 
    {
        ICustomerRepository CustomerRepository { get; }
        IAccountRepository AccountRepository { get; }
        void Begin();
        void Commit();
        void Rollback();
    }
}
