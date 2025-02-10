
using AutoMapper;
using CustomersService.Application.Exceptions;
using CustomersService.Application.Integrations;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Application.Services
{
    public class AccountService(
        IAccountRepository accountRepository,
        IMapper mapper,
        CustomerUtils customerUtils,
        AccountUtils accountUtils) : IAccountService
    {
        private readonly CommonHttpClient _httpClient;

        public AccountService(
            IAccountRepository accountRepository,
            IMapper mapper,
            CustomerUtils customerUtils,
            AccountUtils accountUtils, 
            HttpMessageHandler? handler = null): 
            this(accountRepository, mapper, customerUtils, accountUtils)
        {
            _httpClient = new("http://194.147.90.249:9091/api/v1/accounts", handler);
        }

        public async Task<Guid> CreateAsync(AccountCreationModel accountToCreate)
        {
            var customerDTO = await customerUtils.GetByIdAsync(accountToCreate.CustomerId);

            if(customerDTO.Role == Role.Admin || customerDTO.Role == Role.Unknown)
                throw new EntityConflictException($"Role of customer with id {accountToCreate.CustomerId} is not correct.");

            if(customerDTO.IsDeactivated)
                throw new EntityConflictException($"Customer with id {accountToCreate.CustomerId} is deactivated.");

            var accountDTO = await accountRepository.GetByConditionAsync(a => 
            a.Currency == accountToCreate.Currency && a.CustomerId == accountToCreate.CustomerId);

            if (accountDTO != null)
                throw new EntityConflictException($"Customer with id {accountToCreate.CustomerId} already has an account with currency {accountToCreate.Currency}.");

            if(customerDTO.Role == Role.Regular 
                && accountToCreate.Currency != Currency.USD
                && accountToCreate.Currency != Currency.EUR)
                throw new EntityConflictException($"Customer with role {customerDTO.Role} cannot have an account with this currency.");

            var accountToCreateDTO = mapper.Map<Account>(accountToCreate);
            accountToCreateDTO.Customer = customerDTO;

            await accountRepository.CreateAsync(accountToCreateDTO);

            return accountToCreateDTO.Id;
        }

        public async Task<List<AccountInfoModel>> GetAllByCustomerIdAsync(Guid customerId)
        {
            var accountDTOs = await accountRepository.GetAllByConditionAsync(a => a.CustomerId == customerId);

            return mapper.Map<List<AccountInfoModel>>(accountDTOs);
        }

        public async Task<AccountFullInfoModel> GetFullInfoByIdAsync(Guid id)
        {
            var accountDTO = await accountUtils.GetByIdAsync(id);

            var account = mapper.Map<AccountFullInfoModel>(accountDTO);

            var accountBalanceModel = await _httpClient.SendGetRequestAsync<BalanceResponse>($"/{id}/balance");

            account.Balance = accountBalanceModel.Balance; 

            return account;
        }

        public async Task<List<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id)
        {
           return await _httpClient.SendGetRequestAsync<List<TransactionResponse>>($"/{id}/transactions");
        }

        public async Task DeactivateAsync(Guid id)
        {
            var accountDTO = await accountUtils.GetByIdAsync(id);

            if (accountDTO.Currency == Currency.RUB)
                throw new EntityConflictException($"Account with currency {Currency.RUB} cannot be deactivated.");

            await accountRepository.DeactivateAsync(accountDTO);
        }

        public async Task ActivateAsync(Guid id)
        {
            var accountDTO = await accountUtils.GetByIdAsync(id);

            await accountRepository.ActivateAsync(accountDTO);
        }
    }
}
