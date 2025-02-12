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
        logger.LogInformation("Registering customer with email {Email}", customerToRegister.Email);
        logger.LogTrace("Customer registration data: {@CustomerToRegister}", customerToRegister);

        var customer = await customerRepository.GetByConditionAsync(c => c.Email == customerToRegister.Email);
        logger.LogTrace("Checked for existing customer with email {Email}: {@Customer}", customerToRegister.Email, customer);

        if (customer != null)
        {
            logger.LogWarning("Customer with email {Email} already exists", customerToRegister.Email);
            throw new EntityConflictException($"Customer with email {customerToRegister.Email} already exists.");
        }

        var customerDTO = mapper.Map<Customer>(customerToRegister);
        customerDTO.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(customerToRegister.Password);
        customerDTO.Role = Role.Regular;
        logger.LogTrace("Mapped CustomerRegistrationModel to Customer: {@CustomerDTO}", customerDTO);

        var accountDTO = new Account() { Currency = Currency.RUB };
        logger.LogTrace("Created new account with default currency RUB: {@AccountDTO}", accountDTO);

        try
        {
            await customerUnitOfWork.CreateCustomerAsync(customerDTO, accountDTO);
            logger.LogInformation("Customer registered successfully with ID {CustomerId}", customerDTO.Id);
            return customerDTO.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transaction failed while registering customer with email {Email}", customerToRegister.Email);
            customerUnitOfWork.Rollback();
            throw new TransactionFailedException("Transaction failed.");
        }
    }

    public async Task<List<CustomerInfoModel>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        logger.LogInformation("Retrieving all customers with pageNumber {PageNumber} and pageSize {PageSize}", pageNumber, pageSize);

        var customerDTOs = await customerRepository.GetAllAsync(pageNumber ?? 1, pageSize ?? 10);
        logger.LogTrace("Retrieved customer models: {@CustomerDTOs}", customerDTOs);

        var customers = mapper.Map<List<CustomerInfoModel>>(customerDTOs);
        logger.LogTrace("Mapped CustomerDTOs to CustomerInfoModels: {@Customers}", customers);

        logger.LogInformation("Successfully retrieved {Count} customers", customers.Count);
        return customers;
    }

    public async Task<CustomerFullInfoModel> GetFullInfoByIdAsync(Guid id)
    {
        logger.LogInformation("Retrieving full customer info for customer {CustomerId}", id);

        var customerDTO = await customerUtils.GetByIdAsync(id);
        logger.LogTrace("Retrieved customer data: {@CustomerDTO}", customerDTO);

        var customer = mapper.Map<CustomerFullInfoModel>(customerDTO);
        logger.LogTrace("Mapped CustomerDTO to CustomerFullInfoModel: {@Customer}", customer);

        logger.LogInformation("Successfully retrieved full customer info for customer {CustomerId}", id);
        return customer;
    }

    public async Task UpdateProfileAsync(Guid id, CustomerUpdateModel customerUpdateModel)
    {
        logger.LogInformation("Updating profile for customer {CustomerId}", id);
        logger.LogTrace("Customer update data: {@CustomerUpdateModel}", customerUpdateModel);

        var customerDTO = await customerUtils.GetByIdAsync(id);
        logger.LogTrace("Retrieved customer data: {@CustomerDTO}", customerDTO);

        if (customerDTO.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", id);
            throw new EntityConflictException($"Customer with id {id} is deactivated.");
        }

        var customerUpdateDTO = mapper.Map<Customer>(customerUpdateModel);
        logger.LogTrace("Mapped CustomerUpdateModel to Customer: {@CustomerUpdateDTO}", customerUpdateDTO);

        await customerRepository.UpdateProfileAsync(customerDTO, customerUpdateDTO);
        logger.LogInformation("Successfully updated profile for customer {CustomerId}", id);
    }

    public async Task UpdatePasswordAsync(Guid id, string newPassword, string currentPassword)
    {
        logger.LogInformation("Updating password for customer {CustomerId}", id);
        logger.LogTrace("New password data: {NewPassword}", newPassword);

        var customerDTO = await customerUtils.GetByIdAsync(id);
        logger.LogTrace("Retrieved customer data: {@CustomerDTO}", customerDTO);

        if (customerDTO.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", id);
            throw new EntityConflictException($"Customer with id {id} is deactivated.");
        }

        if (!CheckPassword(currentPassword, customerDTO.Password))
        {
            logger.LogWarning("The credentials are not correct for customer {CustomerId}", id);
            throw new WrongCredentialsException("The credentials are not correct.");
        }

        var password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
        logger.LogTrace("Hashed new password: {Password}", password);

        await customerRepository.UpdatePasswordAsync(customerDTO, password);
        logger.LogInformation("Successfully updated password for customer {CustomerId}", id);
    }

    public async Task SetManualVipAsync(Guid id, DateTime vipExpirationDate)
    {
        logger.LogInformation("Setting VIP status for customer {CustomerId}", id);
        logger.LogTrace("VIP expiration date: {VipExpirationDate}", vipExpirationDate);

        var customerDTO = await customerUtils.GetByIdAsync(id);
        logger.LogTrace("Retrieved customer data: {@CustomerDTO}", customerDTO);

        if (customerDTO.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", id);
            throw new EntityConflictException($"Customer with id {id} is deactivated.");
        }

        var regularAccounts = new List<Currency> { Currency.RUB, Currency.USD, Currency.EUR };

        var accountsToActivate = await accountRepository.GetAllByConditionAsync(a =>
        a.CustomerId == id
        && !regularAccounts.Contains(a.Currency));
        logger.LogTrace("Accounts to activate: {@AccountsToActivate}", accountsToActivate);

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

        var vipCustomers = await customerRepository.GetAllByConditionAsync(c =>
            vipCustomerIds.Contains(c.Id) && c.Role != Role.VIP);
        logger.LogTrace("Retrieved VIP customers: {@VipCustomers}", vipCustomers);

        var customersWithDueVip = await customerRepository.GetAllByConditionAsync(c =>
            c.Role == Role.VIP
            && !vipCustomerIds.Contains(c.Id)
            && (c.CustomVipDueDate == null ||
            c.CustomVipDueDate < DateTime.Now));
        logger.LogTrace("Retrieved customers with due VIP: {@CustomersWithDueVip}", customersWithDueVip);

        var vipCustomersDictionary = vipCustomers.ToDictionary(c => c, c => Role.VIP);
        var customersWithDueVipDictionary = customersWithDueVip.ToDictionary(c => c, c => Role.Regular);
        var customersWithUpdatedRoles = vipCustomersDictionary.Concat(customersWithDueVipDictionary).ToDictionary();
        logger.LogTrace("Customers with updated roles: {@CustomersWithUpdatedRoles}", customersWithUpdatedRoles);

        var regularAccounts = new List<Currency> { Currency.RUB, Currency.USD, Currency.EUR };

        var accountsToActivate = await accountRepository.GetAllByConditionAsync(a =>
        vipCustomerIds.Contains(a.CustomerId)
        && !regularAccounts.Contains(a.Currency));
        logger.LogTrace("Accounts to activate: {@AccountsToActivate}", accountsToActivate);

        var customerWithDueVipIds = customersWithDueVip.Select(c => c.Id).ToList();
        var accountsToDeactivate = await accountRepository.GetAllByConditionAsync(a =>
        customerWithDueVipIds.Contains(a.CustomerId)
        && !regularAccounts.Contains(a.Currency));
        logger.LogTrace("Accounts to deactivate: {@AccountsToDeactivate}", accountsToDeactivate);

        try
        {
            await customerUnitOfWork.BatchUpdateRoleAsync(customersWithUpdatedRoles, accountsToActivate, accountsToDeactivate);
            logger.LogInformation("Successfully batch updated roles for VIP customers");
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
        logger.LogTrace("Retrieved customer data: {@CustomerDTO}", customerDTO);

        await customerRepository.DeactivateAsync(customerDTO);
        logger.LogInformation("Successfully deactivated customer {CustomerId}", id);
    }

    public async Task ActivateAsync(Guid id)
    {
        logger.LogInformation("Activating customer {CustomerId}", id);

        var customerDTO = await customerUtils.GetByIdAsync(id);
        logger.LogTrace("Retrieved customer data: {@CustomerDTO}", customerDTO);

        await customerRepository.ActivateAsync(customerDTO);
        logger.LogInformation("Successfully activated customer {CustomerId}", id);
    }

    private bool CheckPassword(string passwordToCheck, string passwordHash)
    {
        logger.LogTrace("Checking password");
        return BCrypt.Net.BCrypt.EnhancedVerify(passwordToCheck, passwordHash);
    }
}
