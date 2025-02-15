﻿
using System.Net;
using System.Text.Json;
using AutoMapper;
using CustomersService.Application.Exceptions;
using CustomersService.Application.Integrations;
using CustomersService.Application.Mappings;
using CustomersService.Application.Models;
using CustomersService.Application.Services;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Application.Tests.TestCases;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CustomersService.Application.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<HttpMessageHandler> _messageHandlerMock;
        private readonly Mapper _mapper;
        private readonly Mock<ILogger<CustomerUtils>> _customerUtilsLoggerMock;
        private readonly Mock<ILogger<AccountUtils>> _accountUtilsLoggerMock;
        private readonly Mock<ILogger<AccountService>> _accountServiceLoggerMock;
        private readonly Mock<ILogger<CommonHttpClient>> _commonHttpClientLoggerMock;
        private readonly AccountService _sut;

        public AccountServiceTests()
        {
            _accountRepositoryMock = new();
            _customerRepositoryMock = new();
            _messageHandlerMock = new();
            _customerUtilsLoggerMock = new();
            _accountUtilsLoggerMock = new();
            _commonHttpClientLoggerMock = new();
            _accountServiceLoggerMock = new();
            var config = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile(new AccountApplicationMapperProfile());
            });
            _mapper = new Mapper(config);
            _sut = new(
                _accountRepositoryMock.Object,
                _mapper,
                new CustomerUtils(_customerRepositoryMock.Object, _customerUtilsLoggerMock.Object),
                new AccountUtils(_accountRepositoryMock.Object, _accountUtilsLoggerMock.Object),
                _accountServiceLoggerMock.Object,
                _commonHttpClientLoggerMock.Object,
                _messageHandlerMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_RightRoleActiveCustomerNoExistingAccount_CreateSuccess()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var accountModel = new AccountCreationModel { CustomerId = customerId, Currency = Currency.USD };
            var customer = new Customer { Id = accountModel.CustomerId, Role = Role.Regular, IsDeactivated = false };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);

            // Act
            var result = await _sut.CreateAsync(accountModel);

            // Assert
            _accountRepositoryMock.Verify(t =>
                t.CreateAsync(It.Is<Account>(t => t.CustomerId == customerId && t.Customer == customer)),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateAsync_WrongCustomerRole_EntityConflictExceptionThrown()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var message = $"Role of customer with id {customerId} is not correct.";
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(new Customer() { Id = customerId, Role = Role.Admin });
            var accountModel = new AccountCreationModel { CustomerId = customerId};

            // Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateAsync(accountModel));

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task CreateAsync_DeactivatedCustomer_EntityConflictExceptionThrown()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var message = $"Customer with id {customerId} is deactivated.";
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(new Customer() { Id = customerId, Role = Role.Regular, IsDeactivated = true});
            var accountModel = new AccountCreationModel { CustomerId = customerId };

            // Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateAsync(accountModel));

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task CreateAsync_AccountWithCurrencyAlreadyExists_EntityConflictExceptionThrown()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var accountModel = new AccountCreationModel { CustomerId = customerId, Currency = Currency.USD };
            var message = $"Customer with id {customerId} already has an account with currency {accountModel.Currency}.";
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(new Customer() { Id = customerId, Role = Role.Regular });
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Currency == accountModel.Currency && c.CustomerId == accountModel.CustomerId)).ReturnsAsync(new Account() { CustomerId = customerId, Currency = Currency.USD });
            // Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateAsync(accountModel));

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task CreateAsync_AccountWithCurrencyNotAllowed_EntityConflictExceptionThrown()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var accountModel = new AccountCreationModel { CustomerId = customerId, Currency = Currency.JPY };
            var message = $"Customer with role {Role.Regular} cannot have an account with this currency.";
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(new Customer() { Id = customerId, Role = Role.Regular });
            // Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateAsync(accountModel));

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Theory]
        [MemberData(nameof(AccountServiceTestCases.AccountToCreate), MemberType = typeof(AccountServiceTestCases))]
        public void CreateAsync_ValidModel_MappingSuccess(AccountCreationModel accountModel)
        {
            //Act 
            var account = _mapper.Map<Account>(accountModel);

            //Assert
            account.Should().BeEquivalentTo(accountModel, options => options.ExcludingMissingMembers());
            Assert.Equal(account.Id, Guid.Empty);
            Assert.False(account.IsDeactivated);
            Assert.Null(account.Customer);
            Assert.Equal(DateTime.MinValue, account.DateCreated);
        }

        [Fact]
        public async Task GetAllByCustomerIdAsync_GetSuccess()
        {
            // Act
            var customerId = Guid.NewGuid();
            var result = await _sut.GetAllByCustomerIdAsync(customerId);

            // Assert
            _accountRepositoryMock.Verify(t =>
                t.GetAllByConditionAsync(a => a.CustomerId == customerId),
                Times.Once
            );
        }

        [Theory]
        [MemberData(nameof(AccountServiceTestCases.CustomerAccounts), MemberType = typeof(AccountServiceTestCases))]
        public void GetAllByCustomerIdAsync_ValidModel_MappingSuccess(List<Account> accountDTOs)
        {
            //Act
            var accounts = _mapper.Map<List<AccountInfoModel>>(accountDTOs);

            //Assert
            accounts.Should().BeEquivalentTo(accountDTOs, options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetFullInfoByIdAsync_GetSuccess()
        {
            //arrange
            var accountId = Guid.NewGuid();
            var apiEndpoint = $"/{accountId}/balance";
            var account = new Account() { Id = accountId };
            var accountBalance = new BalanceResponse()
            {
                AccountId = accountId,
                Balance = 10000
            };
            var accountFullInfoModel = new AccountFullInfoModel()
            {
                Id = accountId,
                Balance = accountBalance.Balance
            };
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);

            var response = JsonSerializer.Serialize(accountBalance);

            var mockProtected = HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.OK, response);

            //act 
            var result = await _sut.GetFullInfoByIdAsync(accountId);

            //assert
            accountFullInfoModel.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task GetFullInfoByIdAsync_InternalServerErrorReturned_BadGatewayExceptionThrown()
        {
            //arrange
            var accountId = Guid.NewGuid();
            var apiEndpoint = $"/{accountId}/balance";
            var account = new Account() { Id = accountId };
            var message = "Invalid response from the upstream server.";
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);

            var mockProtected = HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.InternalServerError);

            //act 
            var exception = await Assert.ThrowsAsync<BadGatewayException>(async () => await _sut.GetFullInfoByIdAsync(accountId));

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task GetFullInfoByIdAsync_RequestErrorReturned_ServiceUnavailableExceptionThrown()
        {
            //arrange
            var accountId = Guid.NewGuid();
            var apiEndpoint = $"/{accountId}/balance";
            var account = new Account() { Id = accountId };
            var message = "Request to the service failed.";
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);

            var mockProtected = HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.NotFound);

            //act 
            var exception = await Assert.ThrowsAsync<ServiceUnavailableException>(async () => await _sut.GetFullInfoByIdAsync(accountId));

            // Assert
            Assert.Equal(message, exception.Message);
        }


        [Theory]
        [MemberData(nameof(AccountServiceTestCases.AccountWithFullInfo), MemberType = typeof(AccountServiceTestCases))]
        public void GetFullInfoByIdAsync_ValidModel_MappingSuccess(Account accountDTO)
        {
            //Act 
            var account = _mapper.Map<AccountFullInfoModel>(accountDTO);

            //Assert
            account.Should().BeEquivalentTo(accountDTO, options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task GetTransactionsByAccountIdAsync_GetSuccess()
        {
            //arrange
            var accountId = Guid.NewGuid();
            var apiEndpoint = $"/{accountId}/transactions";
            var transactions = new List<TransactionResponse>() 
            { 
                new TransactionResponse()
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    Amount = 100,
                    Date = DateTime.Now,
                    TransactionType = TransactionType.Deposit
                }
            };

            var response = JsonSerializer.Serialize(transactions);
            var mockProtected = HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.OK, response);

            //act 
            var result = await _sut.GetTransactionsByAccountIdAsync(accountId);

            //assert
            transactions.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task GetTransactionsByAccountIdAsync_InternalServerErrorReturned_BadGatewayExceptionThrown()
        {
            //arrange
            var accountId = Guid.NewGuid();
            var apiEndpoint = $"/{accountId}/transactions";
            var message = "Invalid response from the upstream server.";

            var mockProtected = HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.InternalServerError);

            //act 
            var exception = await Assert.ThrowsAsync<BadGatewayException>(async () => await _sut.GetTransactionsByAccountIdAsync(accountId));

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task GetTransactionsByAccountIdAsync_RequestErrorReturned_ServiceUnavailableExceptionThrown()
        {
            //arrange
            var accountId = Guid.NewGuid();
            var apiEndpoint = $"/{accountId}/transactions";
            var message = "Request to the service failed.";

            var mockProtected = HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.NotFound);

            //act 
            var exception = await Assert.ThrowsAsync<ServiceUnavailableException>(async () => await _sut.GetTransactionsByAccountIdAsync(accountId));

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task DeactivateAsync_NotRubAccount_DeactivateSuccess()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account() { Id = accountId, Currency = Currency.USD};
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(a => a.Id == accountId)).ReturnsAsync(account);

            // Act
            await _sut.DeactivateAsync(accountId);

            // Assert
            _accountRepositoryMock.Verify(t =>
                t.DeactivateAsync(It.Is<Account>(t => t.Id == accountId)),
                Times.Once
            );
        }

        [Fact]
        public async Task DeactivateAsync_RubAccount_EntityConflictExceptionThrown()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var message = $"Account with currency {Currency.RUB} cannot be deactivated.";
            var account = new Account() { Id = accountId, Currency = Currency.RUB};
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(a => a.Id == accountId)).ReturnsAsync(account);
            // Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.DeactivateAsync(accountId));

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task ActivateAsync_ActivateSuccess()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account() { Id = accountId};
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(a => a.Id == accountId)).ReturnsAsync(account);

            // Act
            await _sut.ActivateAsync(accountId);

            // Assert
            _accountRepositoryMock.Verify(t =>
                t.ActivateAsync(It.Is<Account>(t => t.Id == accountId)),
                Times.Once
            );
        }
    }
}
