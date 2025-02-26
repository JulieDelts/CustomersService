using AutoMapper;
using CustomersService.Application.Exceptions;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using MYPBackendMicroserviceIntegrations.Enums;
using MYPBackendMicroserviceIntegrations.Messages;

namespace CustomersService.Application.Services;

public class CustomerService(
        ICustomerRepository customerRepository,
        IAccountRepository accountRepository,
        IMapper mapper,
        CustomerUtils customerUtils,
        ICustomerUnitOfWork customerUnitOfWork,
        IPublishEndpoint publishEndpoint,
        ILogger<CustomerService> logger)
    : ICustomerService
{
    public async Task<Guid> RegisterAsync(CustomerRegistrationModel customerToRegister)
    {
        logger.LogInformation("Registering customer with email {Email}", LoggerMaskHelper.MaskEmail(customerToRegister.Email));

        var customer = await customerRepository.GetByConditionAsync(c => c.Email == customerToRegister.Email);

        if (customer != null)
        {
            logger.LogWarning("Customer with email {Email} already exists", LoggerMaskHelper.MaskEmail(customerToRegister.Email));
            throw new EntityConflictException($"Customer with email {customerToRegister.Email} already exists.");
        }

        var customerDto = mapper.Map<Customer>(customerToRegister);
        customerDto.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(customerToRegister.Password);
        customerDto.Role = Role.Regular;

        var accountDto = new Account() { Currency = Currency.RUB };

        try
        {
            await customerUnitOfWork.CreateCustomerAsync(customerDto, accountDto);

            var customerMessage = mapper.Map<CustomerMessage>(customerDto);
            var accountMessage = mapper.Map<AccountMessage>(accountDto);

            var message = new CustomerWithAccountMessage() { Customer = customerMessage, Account = accountMessage };

            logger.LogInformation("Sending customer and account update with customer id {customerId} and account id {accountId} to RabbitMq", customerDto.Id, accountDto.Id);

            await publishEndpoint.Publish(message);

            logger.LogInformation("Sent customer and account update with customer id {customerId} and account id {accountId} to RabbitMq", customerDto.Id, accountDto.Id);

            logger.LogInformation("Customer registered successfully with ID {CustomerId}", customerDto.Id);

            return customerDto.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transaction failed while registering customer with email {Email}", LoggerMaskHelper.MaskEmail(customerToRegister.Email));
            customerUnitOfWork.Rollback();
            throw new TransactionFailedException("Transaction failed.");
        }
    }

    public async Task<string> AuthenticateAsync(string email, string password)
    {
        logger.LogInformation("Authenticating customer with email {Email}", LoggerMaskHelper.MaskEmail(email));

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

        var customerDtos = await customerRepository.GetAllAsync(pageNumber ?? 1, pageSize ?? 10);
        var customers = mapper.Map<List<CustomerInfoModel>>(customerDtos);

        logger.LogInformation("Successfully retrieved {Count} customers", customers.Count);

        return customers;
    }

    public async Task<CustomerFullInfoModel> GetFullInfoByIdAsync(Guid id)
    {
        logger.LogInformation("Retrieving full customer info for customer {CustomerId}", id);

        var customerDto = await customerUtils.GetByIdAsync(id);
        var customer = mapper.Map<CustomerFullInfoModel>(customerDto);

        logger.LogInformation("Successfully retrieved full customer info for customer {CustomerId}", id);

        return customer;
    }

    public async Task UpdateProfileAsync(Guid id, CustomerUpdateModel customerUpdateModel)
    {
        logger.LogInformation("Updating profile for customer {CustomerId}", id);

        var customerDto = await customerUtils.GetByIdAsync(id);
        logger.LogDebug("Retrieved customer data: {@CustomerDto}", customerDto);

        if (customerDto.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", id);
            throw new EntityConflictException($"Customer with id {id} is deactivated.");
        }

        var customerUpdateDto = mapper.Map<Customer>(customerUpdateModel);

        await customerRepository.UpdateProfileAsync(customerDto, customerUpdateDto);
        await PublishCustomerUpdateAsync(customerDto);

        logger.LogInformation("Successfully updated profile for customer {CustomerId}", id);
    }

    public async Task UpdatePasswordAsync(Guid id, string newPassword, string currentPassword)
    {
        logger.LogInformation("Updating password for customer {CustomerId}", id);

        var customerDto = await customerUtils.GetByIdAsync(id);

        if (customerDto.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", id);
            throw new EntityConflictException($"Customer with id {id} is deactivated.");
        }

        if (!customerUtils.CheckPassword(currentPassword, customerDto.Password))
        {
            logger.LogWarning("The credentials are not correct for customer {CustomerId}", id);
            throw new WrongCredentialsException("The credentials are not correct.");
        }

        var password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);

        await customerRepository.UpdatePasswordAsync(customerDto, password);
        await PublishCustomerUpdateAsync(customerDto);

        logger.LogInformation("Successfully updated password for customer {CustomerId}", id);
    }

    public async Task SetManualVipAsync(Guid id, DateTime vipExpirationDate)
    {
        logger.LogInformation("Setting VIP status for customer {CustomerId}", id);

        var customerDto = await customerUtils.GetByIdAsync(id);

        if (customerDto.IsDeactivated)
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
            await customerUnitOfWork.SetManualVipAsync(customerDto, vipExpirationDate, accountsToActivate);
            await PublishCustomerUpdateAsync(customerDto);

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

        try
        {
            await customerUnitOfWork.BatchUpdateRoleAsync(vipCustomerIds);

            logger.LogInformation("Sending role update with ids {ids} to RabbitMq", string.Join(", ", vipCustomerIds));

            var message = new CustomerRoleUpdateIdsReportingMessage() { VipCustomerIds = vipCustomerIds };

            await publishEndpoint.Publish(message);

            logger.LogInformation("Sent role update with ids {ids} to RabbitMq", string.Join(", ", vipCustomerIds));
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

        var customerDto = await customerUtils.GetByIdAsync(id);

        await customerRepository.DeactivateAsync(customerDto);
        await PublishCustomerUpdateAsync(customerDto);

        logger.LogInformation("Successfully deactivated customer {CustomerId}", id);
    }

    public async Task ActivateAsync(Guid id)
    {
        logger.LogInformation("Activating customer {CustomerId}", id);

        var customerDto = await customerUtils.GetByIdAsync(id);

        await customerRepository.ActivateAsync(customerDto);
        await PublishCustomerUpdateAsync(customerDto);

        logger.LogInformation("Successfully activated customer {CustomerId}", id);
    }

    private async Task PublishCustomerUpdateAsync(Customer customer)
    {
        var message = mapper.Map<CustomerMessage>(customer);

        logger.LogInformation("Sending customer update with id {id} to RabbitMq", customer.Id);

        await publishEndpoint.Publish(message);

        logger.LogInformation("Sent customer update with id {id} to RabbitMq", customer.Id);
    }
}
