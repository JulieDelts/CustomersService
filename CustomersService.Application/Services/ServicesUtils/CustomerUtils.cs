using CustomersService.Application.Exceptions;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.Extensions.Logging;

namespace CustomersService.Application.Services.ServicesUtils;

public class CustomerUtils(
    ICustomerRepository customerRepository,
    ILogger<CustomerUtils> logger)
{
    public async Task<Customer> GetByIdAsync(Guid id)
    {
        logger.LogDebug("Retrieving customer with ID {CustomerId}", id);

        var customerDTO = await customerRepository.GetByConditionAsync(c => c.Id == id);
        logger.LogTrace("Retrieved customer data: {@CustomerDTO}", customerDTO);

        if (customerDTO == null)
        {
            logger.LogWarning("Customer with id {CustomerId} was not found", id);
            throw new EntityNotFoundException($"Customer with id {id} was not found.");
        }

        logger.LogDebug("Successfully retrieved customer with ID {CustomerId}", id);
        return customerDTO;
    }
}
