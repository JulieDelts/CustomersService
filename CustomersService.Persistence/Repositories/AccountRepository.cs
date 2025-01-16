using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;


namespace CustomersService.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public void CreateAccount(Guid id)
        {
            throw new NotImplementedException();
        }

        public void DeleteAccount(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteAccount(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Account> GetAccountById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Account> GetAccountById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void UpdateAccount(Account account)
        {
            throw new NotImplementedException();
        }

        void IAccountRepository.DeactivateAccount(Account account)
        {
            throw new NotImplementedException();
        }

        List<Account> IAccountRepository.GetAllAccounts()
        {
            throw new NotImplementedException();
        }
    }
}
