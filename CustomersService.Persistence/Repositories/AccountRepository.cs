using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public async Task<Guid> CreateAsync(Account account)
        {
            return Guid.NewGuid();
        }

        public async Task<List<Account>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Account> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task ActivateAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public async Task DeactivateAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
