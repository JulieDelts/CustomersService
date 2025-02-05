
using CustomersService.Application.Models;

namespace CustomersService.Application.Interfaces
{
    public interface IAccountService
    {
        Task ActivateAsync(Guid id);
        Task<Guid> CreateAsync(AccountCreationModel accountToCreate);
        Task DeactivateAsync(Guid id);
        Task<List<AccountInfoModel>> GetAllByCustomerIdAsync(Guid customerId);
        Task<AccountFullInfoModel> GetFullInfoByIdAsync(Guid id);
    }
}
