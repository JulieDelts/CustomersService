using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces;

public interface IAccountRepository : IBaseRepository<Account>, IDeactivatable<Account>
{
    Task ActivateAsync(List<Account> accounts);
}

