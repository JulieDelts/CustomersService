using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces
{
    public interface IAccountRepository
    {
        Task<Guid> CreateAsync(Account account);
        Task<List<Account>> GetAllAsync();
        Task<Account> GetByIdAsync(Guid id);
        Task ActivateAsync(Account account);
        Task DeactivateAsync(Account account);
        Task DeleteAsync(Account account);
    }
}

