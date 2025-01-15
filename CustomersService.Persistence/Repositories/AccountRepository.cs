using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomersService.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public void CreateAccount(Account account)
        {
            throw new NotImplementedException();
        }

        public void DeleteAccount(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Account> GetAccountById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            throw new NotImplementedException();
        }

        public void UpdateAccount(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
