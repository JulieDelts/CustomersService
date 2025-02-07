using AutoMapper;
using CustomersService.Application.Exceptions;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Application.Services
{
    public class CustomerService(
        ICustomerRepository customerRepository,
        IAccountRepository accountRepository,
        IMapper mapper,
        CustomerUtils customerUtils,
        ICustomerUnitOfWork customerUnitOfWork)
        : ICustomerService
    {
        public async Task<Guid> RegisterAsync(CustomerRegistrationModel customerToRegister)
        {
            var customer = await customerRepository.GetByConditionAsync(c => c.Email == customerToRegister.Email);

            if (customer != null)
                throw new EntityConflictException($"Customer with email {customerToRegister.Email} already exists.");

            var customerDTO = mapper.Map<Customer>(customerToRegister);
            customerDTO.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(customerToRegister.Password);
            customerDTO.Role = Role.Regular;

            var accountDTO = new Account() { Currency = Currency.RUB };

            try
            {
                await customerUnitOfWork.CreateCustomerAsync(customerDTO, accountDTO);
                return customerDTO.Id;
            }
            catch
            {
                customerUnitOfWork.Rollback();
                throw new TransactionFailedException("Transaction failed.");
            }
        }

        public async Task<List<CustomerInfoModel>> GetAllAsync(int skip, int take)
        {
            var customerDTOs = await customerRepository.GetAllAsync(skip, take);

            return mapper.Map<List<CustomerInfoModel>>(customerDTOs);
        }

        public async Task<CustomerFullInfoModel> GetFullInfoByIdAsync(Guid id)
        {
            var customerDTO = await customerUtils.GetByIdAsync(id);

            return mapper.Map<CustomerFullInfoModel>(customerDTO);
        }

        public async Task UpdateProfileAsync(Guid id, CustomerUpdateModel customerUpdateModel)
        {
            var customerDTO = await customerUtils.GetByIdAsync(id);

            if (customerDTO.IsDeactivated)
                throw new EntityConflictException($"Customer with id {id} is deactivated.");

            var customerUpdateDTO = mapper.Map<Customer>(customerUpdateModel);

            await customerRepository.UpdateProfileAsync(customerDTO, customerUpdateDTO);
        }

        public async Task UpdatePasswordAsync(Guid id, string newPassword, string currentPassword)
        {
            var customerDTO = await customerUtils.GetByIdAsync(id);

            if (customerDTO.IsDeactivated)
                throw new EntityConflictException($"Customer with id {id} is deactivated.");

            if (!CheckPassword(currentPassword, customerDTO.Password))
                throw new WrongCredentialsException("The credentials are not correct.");

            var password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);

            await customerRepository.UpdatePasswordAsync(customerDTO, password);
        }

        public async Task SetManualVipAsync(Guid id, DateTime vipExpirationDate)
        {
            var customerDTO = await customerUtils.GetByIdAsync(id);

            if (customerDTO.IsDeactivated)
                throw new EntityConflictException($"Customer with id {id} is deactivated.");

            var regularAccounts = new List<Currency> { Currency.RUB, Currency.USD, Currency.EUR };

            var accountsToActivate = await accountRepository.GetAllByConditionAsync(a =>
            a.CustomerId == id
            && !regularAccounts.Contains(a.Currency));

            try
            {
                await customerUnitOfWork.SetManualVipAsync(customerDTO, vipExpirationDate, accountsToActivate);
            }
            catch
            {
                customerUnitOfWork.Rollback();
                throw new TransactionFailedException("Transaction failed.");
            }
        }

        public async Task BatchUpdateRoleAsync(List<Guid> vipCustomerIds)
        {
            var vipCustomers = await customerRepository.GetAllByConditionAsync(c => 
                vipCustomerIds.Contains(c.Id) && c.Role != Role.VIP);

            var customersWithDueVip = await customerRepository.GetAllByConditionAsync(c =>
                c.Role == Role.VIP
                && !vipCustomerIds.Contains(c.Id)
                && (c.CustomVipDueDate == null ||
                c.CustomVipDueDate < DateTime.Now));

            var vipCustomersDictionary = vipCustomers.ToDictionary(c => c, c => Role.VIP);
            var customersWithDueVipDictionary = customersWithDueVip.ToDictionary(c => c, c => Role.Regular);
            var customersWithUpdatedRoles = vipCustomersDictionary.Concat(customersWithDueVipDictionary).ToDictionary();

            var regularAccounts = new List<Currency> { Currency.RUB, Currency.USD, Currency.EUR };

            var accountsToActivate = await accountRepository.GetAllByConditionAsync(a =>
            vipCustomerIds.Contains(a.CustomerId)
            && !regularAccounts.Contains(a.Currency));

            var customerWithDueVipIds = customersWithDueVip.Select(c => c.Id).ToList();
            var accountsToDeactivate = await accountRepository.GetAllByConditionAsync(a =>
            customerWithDueVipIds.Contains(a.CustomerId)
            && !regularAccounts.Contains(a.Currency));

            try
            {
               await customerUnitOfWork.BatchUpdateRoleAsync(customersWithUpdatedRoles, accountsToActivate, accountsToDeactivate);
            }
            catch
            {
                customerUnitOfWork.Rollback();
                throw new TransactionFailedException("Transaction failed.");
            }
        }

        public async Task DeactivateAsync(Guid id)
        {
            var customerDTO = await customerUtils.GetByIdAsync(id);

            await customerRepository.DeactivateAsync(customerDTO);
        }

        public async Task ActivateAsync(Guid id)
        {
            var customerDTO = await customerUtils.GetByIdAsync(id);

            await customerRepository.ActivateAsync(customerDTO);
        }

        private bool CheckPassword(string passwordToCheck, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(passwordToCheck, passwordHash);
        }
    }
}
