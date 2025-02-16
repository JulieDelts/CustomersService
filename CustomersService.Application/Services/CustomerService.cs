using AutoMapper;
using CustomersService.Application.Exceptions;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.Extensions.Logging;

namespace CustomersService.Application.Services;

public class CustomerService(
        ICustomerRepository customerRepository,
        IAccountRepository accountRepository,
        IMapper mapper,
        CustomerUtils customerUtils,
        ICustomerUnitOfWork customerUnitOfWork,
        ILogger<CustomerService> logger)
    : ICustomerService
{
    public async Task<Guid> RegisterAsync(CustomerRegistrationModel customerToRegister)
    {
        logger.LogInformation("Registering customer with email {Email}", MaskEmail(customerToRegister.Email));

        var customer = await customerRepository.GetByConditionAsync(c => c.Email == customerToRegister.Email);

        if (customer != null)
        {
            logger.LogWarning("Customer with email {Email} already exists", MaskEmail(customerToRegister.Email));
            throw new EntityConflictException($"Customer with email {customerToRegister.Email} already exists.");
        }

        var customerDTO = mapper.Map<Customer>(customerToRegister);
        customerDTO.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(customerToRegister.Password);
        customerDTO.Role = Role.Regular;

        var accountDTO = new Account() { Currency = Currency.RUB };

        try
        {
            await customerUnitOfWork.CreateCustomerAsync(customerDTO, accountDTO);
            logger.LogInformation("Customer registered successfully with ID {CustomerId}", customerDTO.Id);
            return customerDTO.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transaction failed while registering customer with email {Email}", MaskEmail(customerToRegister.Email));
            customerUnitOfWork.Rollback();
            throw new TransactionFailedException("Transaction failed.");
        }
    }

    public async Task<string> AuthenticateAsync(string email, string password)
    {
        logger.LogInformation("Authenticating customer with email {Email}", MaskEmail(email));

        var customer = await customerRepository.GetByConditionAsync(c => c.Email == email);

        if (customer != null && customerUtils.CheckPassword(password, customer.Password))
        {
            return customerUtils.GenerateToken(customer);
        }
        else
        {
            throw new WrongCredentialsException("The credentials are not correct.");
        }
    }

    public async Task<List<CustomerInfoModel>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        logger.LogInformation("Retrieving all customers with pageNumber {PageNumber} and pageSize {PageSize}", pageNumber, pageSize);

        var customerDTOs = await customerRepository.GetAllAsync(pageNumber ?? 1, pageSize ?? 10);
        var customers = mapper.Map<List<CustomerInfoModel>>(customerDTOs);

        logger.LogInformation("Successfully retrieved {Count} customers", customers.Count);
        return customers;
    }

    public async Task<CustomerFullInfoModel> GetFullInfoByIdAsync(Guid id)
    {
        logger.LogInformation("Retrieving full customer info for customer {CustomerId}", id);

        var customerDTO = await customerUtils.GetByIdAsync(id);
        var customer = mapper.Map<CustomerFullInfoModel>(customerDTO);

        logger.LogInformation("Successfully retrieved full customer info for customer {CustomerId}", id);
        return customer;
    }

    public async Task UpdateProfileAsync(Guid id, CustomerUpdateModel customerUpdateModel)
    {
        logger.LogInformation("Updating profile for customer {CustomerId}", id);

        var customerDTO = await customerUtils.GetByIdAsync(id);
        logger.LogDebug("Retrieved customer data: {@CustomerDTO}", customerDTO);

        if (customerDTO.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", id);
            throw new EntityConflictException($"Customer with id {id} is deactivated.");
        }

        var customerUpdateDTO = mapper.Map<Customer>(customerUpdateModel);

        await customerRepository.UpdateProfileAsync(customerDTO, customerUpdateDTO);
        logger.LogInformation("Successfully updated profile for customer {CustomerId}", id);
    }

    public async Task UpdatePasswordAsync(Guid id, string newPassword, string currentPassword)
    {
        logger.LogInformation("Updating password for customer {CustomerId}", id);

        var customerDTO = await customerUtils.GetByIdAsync(id);

        if (customerDTO.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", id);
            throw new EntityConflictException($"Customer with id {id} is deactivated.");
        }

        if (!customerUtils.CheckPassword(currentPassword, customerDTO.Password))
        {
            logger.LogWarning("The credentials are not correct for customer {CustomerId}", id);
            throw new WrongCredentialsException("The credentials are not correct.");
        }

        var password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);

        await customerRepository.UpdatePasswordAsync(customerDTO, password);
        logger.LogInformation("Successfully updated password for customer {CustomerId}", id);
    }

    public async Task SetManualVipAsync(Guid id, DateTime vipExpirationDate)
    {
        logger.LogInformation("Setting VIP status for customer {CustomerId}", id);

        var customerDTO = await customerUtils.GetByIdAsync(id);

        if (customerDTO.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", id);
            throw new EntityConflictException($"Customer with id {id} is deactivated.");
        }

        var regularAccounts = new List<Currency> { Currency.RUB, Currency.USD, Currency.EUR };

        var accountsToActivate = await accountRepository.GetAllByConditionAsync(a =>
        a.CustomerId == id
        && !regularAccounts.Contains(a.Currency));

        try
        {
            await customerUnitOfWork.SetManualVipAsync(customerDTO, vipExpirationDate, accountsToActivate);
            logger.LogInformation("Successfully set VIP status for customer {CustomerId}", id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transaction failed while setting VIP status for customer {CustomerId}", id);
            customerUnitOfWork.Rollback();
            throw new TransactionFailedException("Transaction failed.");
        }
    }

    public async Task BatchUpdateRoleAsync(List<Guid> vipCustomerIds)
    {
        logger.LogInformation("Batch updating roles for VIP customers");
        logger.LogTrace("VIP customer IDs: {@VipCustomerIds}", vipCustomerIds);

        try
        {
            await customerUnitOfWork.BatchUpdateRoleAsync(vipCustomerIds);
            logger.LogInformation("Successfully batch updated roles for customers");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transaction failed while batch updating roles for VIP customers");
            customerUnitOfWork.Rollback();
            throw new TransactionFailedException("Transaction failed.");
        }
    }

    public async Task DeactivateAsync(Guid id)
    {
        logger.LogInformation("Deactivating customer {CustomerId}", id);

        var customerDTO = await customerUtils.GetByIdAsync(id);

        await customerRepository.DeactivateAsync(customerDTO);
        logger.LogInformation("Successfully deactivated customer {CustomerId}", id);
    }

    public async Task ActivateAsync(Guid id)
    {
        logger.LogInformation("Activating customer {CustomerId}", id);

        var customerDTO = await customerUtils.GetByIdAsync(id);

        await customerRepository.ActivateAsync(customerDTO);
        logger.LogInformation("Successfully activated customer {CustomerId}", id);
    }

    private string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex <= 1)
        {
            return email;
        }

        var maskedEmail = email.Substring(0, 1) + new string('*', atIndex - 1) + email.Substring(atIndex);
        return maskedEmail;
    }
}
