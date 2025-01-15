using CustomersService.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomersService.Persistence.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountById(int id);
        IEnumerable<Account> GetAllAccounts();
        void CreateAccount(Account account);
        void UpdateAccount(Account account);
        void DeleteAccount(int id);
    }
}

