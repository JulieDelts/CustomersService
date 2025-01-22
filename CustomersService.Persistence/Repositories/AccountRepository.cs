using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Persistence.Repositories
{
    public class AccountRepository(CustomerServiceDbContext context) : BaseRepository<Account>(context), IAccountRepository
    {
        public override async Task ActivateAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public override async Task DeactivateAsync(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
