
using CustomersService.Application.Models;

namespace CustomersService.Application.Interfaces
{
    public interface ICustomerService
    {
        Task ActivateAsync(Guid id);
        Task DeactivateAsync(Guid id);
        Task<List<CustomerInfoModel>> GetAllAsync(int skip, int take);
        Task<CustomerFullInfoModel> GetFullInfoByIdAsync(Guid id);
        Task<Guid> RegisterAsync(CustomerRegistrationModel customerToRegister);
        Task SetManualVipAsync(Guid id, DateTime vipExpirationDate);
        Task UpdatePasswordAsync(Guid id, string newPassword, string currentPassword);
        Task UpdateProfileAsync(Guid id, CustomerUpdateModel customerUpdateModel);
        Task BatchUpdateRoleAsync(List<Guid> vipCustomerIds);
    }
}
