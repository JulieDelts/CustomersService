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
                customerUnitOfWork.Begin();

                await customerRepository.CreateAsync(customerDTO);
                accountDTO.Customer = customerDTO;
                await accountRepository.CreateAsync(accountDTO);

                customerUnitOfWork.Commit();

                return customerDTO.Id;
            }
            catch
            {
                customerUnitOfWork.Rollback();
                throw new TransactionFailedException("Transaction failed.");
            }
        }

        public async Task<List<CustomerInfoModel>> GetAllAsync()
        {
            var customerDTOs = await customerRepository.GetAllAsync();

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

        public async Task DeleteAsync(Guid id)
        {
            var customerDTO = await customerUtils.GetByIdAsync(id);

            var accountDTO = await accountRepository.GetByConditionAsync(a => 
            a.CustomerId == id && a.Currency == Currency.RUB);

            if (accountDTO == null)
                throw new EntityNotFoundException($"Account with customer id {id} and currency {Currency.RUB} was not found.");

            try
            {
                customerUnitOfWork.Begin();

                await accountRepository.DeleteAsync(accountDTO);
                await customerRepository.DeleteAsync(customerDTO);

                customerUnitOfWork.Commit();
            }
            catch
            {
                customerUnitOfWork.Rollback();
                throw new TransactionFailedException("Transaction failed.");
            }
        }

        private bool CheckPassword(string passwordToCheck, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(passwordToCheck, passwordHash);
        }
    }
}
