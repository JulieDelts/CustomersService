
using CustomersService.Application.Exceptions;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Application.Services.ServicesUtils
{
    public class AccountUtils(IAccountRepository accountRepository)
    {
        public async Task<Account> GetByIdAsync(Guid id)
        {
            var accountDTO = await accountRepository.GetByConditionAsync(a => a.Id == id);

            if (accountDTO == null)
                throw new EntityNotFoundException($"Account with id {id} was not found.");

            return accountDTO;
        }
    }
}
