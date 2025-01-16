using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountById(Guid id);
        List<Account> GetAllAccounts();
        void CreateAccount(Guid id);
        void UpdateAccount(Account account);
        void DeactivateAccount(Account account);
        void DeleteAccount(Guid id);
    }
}

