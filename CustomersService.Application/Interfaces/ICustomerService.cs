
using CustomersService.Application.Models;

namespace CustomersService.Application.Interfaces
{
    public interface ICustomerService
    {
        Task ActivateAsync(Guid id);
        Task DeactivateAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task<List<CustomerInfoModel>> GetAllAsync();
        Task<CustomerFullInfoModel> GetFullInfoByIdAsync(Guid id);
        Task<Guid> RegisterAsync(CustomerRegistrationModel customerToRegister);
        Task UpdatePasswordAsync(Guid id, string newPassword, string currentPassword);
        Task UpdateProfileAsync(Guid id, CustomerUpdateModel customerUpdateModel);
    }
}
