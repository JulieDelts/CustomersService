using CustomersService.Application.Exceptions;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Application.Services.ServicesUtils
{
    public class CustomerUtils(ICustomerRepository customerRepository)
    {
        public async Task<Customer> GetByIdAsync(Guid id)
        {
            var customerDTO = await customerRepository.GetByConditionAsync(c => c.Id == id);

            if (customerDTO == null)
                throw new EntityNotFoundException($"Customer with id {id} was not found.");

            return customerDTO;
        }

    }
}
