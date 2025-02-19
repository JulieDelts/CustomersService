
using CustomersService.Application.Exceptions;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Services;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core;
using CustomersService.Core.DTOs.Requests;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CustomersService.Application.Tests
{
    public class TransactionServiceTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ILogger<CustomerUtils>> _customerUtilsLoggerMock;
        private readonly Mock<ILogger<AccountUtils>> _accountUtilsLoggerMock;
        private readonly Mock<ILogger<TransactionService>> _transactionServiceLoggerMock;
        private readonly Mock<ICommonHttpClient> _commonHttpClientMock;
        private readonly Mock<IOptions<TransactionStoreAPIConnectionStrings>> _optionsMock;
        private TransactionService _sut;
  
        public TransactionServiceTests()
        {
            _accountRepositoryMock = new();
            _customerRepositoryMock = new();
            _customerUtilsLoggerMock = new Mock<ILogger<CustomerUtils>>();
            _accountUtilsLoggerMock = new Mock<ILogger<AccountUtils>>();
            _transactionServiceLoggerMock = new Mock<ILogger<TransactionService>>();
            _commonHttpClientMock = new();
            _optionsMock = new();

            _sut = new TransactionService(
                new CustomerUtils(_customerRepositoryMock.Object, _customerUtilsLoggerMock.Object),
                new AccountUtils(_accountRepositoryMock.Object, _accountUtilsLoggerMock.Object),
                _transactionServiceLoggerMock.Object,
                _commonHttpClientMock.Object,
                _optionsMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_GetSuccess()
        {
            //Arrange
            var id = Guid.NewGuid();
            var transaction = new TransactionResponse()
            {
                Id = id,
                AccountId = Guid.NewGuid(),
                Amount = 100,
                Date = DateTime.Now,
                TransactionType = TransactionType.Deposit
            };
            _commonHttpClientMock.Setup(t => t.SendGetRequestAsync<TransactionResponse>($"{null}/{id}")).ReturnsAsync(transaction);

            //Act 
            var result = await _sut.GetByIdAsync(id);

            //Assert
            transaction.Should().BeEquivalentTo(result);
            _commonHttpClientMock.Verify(t =>
            t.SendGetRequestAsync<TransactionResponse>(It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateSimpleTransactionAsync_CreateSuccess()
        {
            //Arrange
            var accountId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var transactionId = Guid.NewGuid();
            var transactionType = TransactionType.Deposit;
            var request = new CreateTransactionRequest() { AccountId = accountId };
            var account = new Account() { CustomerId = customerId, Currency = Currency.USD };
            var customer = new Customer();
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
            _commonHttpClientMock.Setup(t => t.SendPostRequestAsync<CreateTransactionRequest, Guid>($"{null}/deposit", request)).ReturnsAsync(transactionId);

            //Act 
            var result = await _sut.CreateSimpleTransactionAsync(request, transactionType);

            //Assert
            Assert.Equal(transactionId, result);
            _commonHttpClientMock.Verify(t =>
            t.SendPostRequestAsync<CreateTransactionRequest, Guid>(It.IsAny<string>(),
                It.Is<CreateTransactionRequest>(t => t.AccountId == accountId)),
                Times.Once);
        }

        [Fact]
        public async Task CreateSimpleTransactionAsync_WrongTransactionType_EntityConflictExceptionThrown()
        {

            //Arrange
            var accountId = Guid.NewGuid();
            var message = "TransactionType is not correct.";
            var transactionType = TransactionType.Transfer;
            var request = new CreateTransactionRequest() { AccountId = accountId };

            //Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateSimpleTransactionAsync(request, transactionType));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task CreateSimpleTransactionAsync_AccountDeactivated_EntityConflictExceptionThrown()
        {

            //Arrange
            var accountId = Guid.NewGuid();
            var message = $"Account with id {accountId} is deactivated.";
            var transactionType = TransactionType.Deposit;
            var account = new Account() { Id = accountId, IsDeactivated = true };
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);
            var request = new CreateTransactionRequest() { AccountId = accountId };

            //Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateSimpleTransactionAsync(request, transactionType));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task CreateSimpleTransactionAsync_WrongCurrency_EntityConflictExceptionThrown()
        {

            //Arrange
            var accountId = Guid.NewGuid();
            var transactionType = TransactionType.Deposit;
            var message = $"Deposit and withdraw transactions are only allowed for accounts with currencies {Currency.RUB}, {Currency.USD}.";
            var account = new Account() { Id = accountId, Currency = Currency.JPY };
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);
            var request = new CreateTransactionRequest() { AccountId = accountId };

            //Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateSimpleTransactionAsync(request, transactionType));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task CreateSimpleTransactionAsync_CustomerDeactivated_EntityConflictExceptionThrown()
        {

            //Arrange
            var accountId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var transactionType = TransactionType.Deposit;
            var message = $"Customer with id {customerId} is deactivated.";
            var account = new Account() { Id = accountId, Currency = Currency.USD, CustomerId = customerId };
            var customer = new Customer() { Id = customerId, IsDeactivated = true };
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
            var request = new CreateTransactionRequest() { AccountId = accountId };

            //Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateSimpleTransactionAsync(request, transactionType));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task CreateTransferTransactionAsync_CreateSuccess()
        {
            //arrange
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var idList = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() };
            var request = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, FromCurrency = Currency.EUR, ToCurrency = Currency.RUB };
            var fromAccount = new Account() { Id = fromAccountId, CustomerId = customerId, Currency = request.FromCurrency };
            var toAccount = new Account() { Id = toAccountId, CustomerId = customerId, Currency = request.ToCurrency };
            var customer = new Customer() { Id = customerId };
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == fromAccountId)).ReturnsAsync(fromAccount);
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == toAccountId)).ReturnsAsync(toAccount);
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
            _commonHttpClientMock.Setup(t => t.SendPostRequestAsync<CreateTransferTransactionRequest, List<Guid>>($"{null}/transfer", request)).ReturnsAsync(idList);

            //act 
            var result = await _sut.CreateTransferTransactionAsync(request);

            // Assert
            idList.Should().BeEquivalentTo(result);
            _commonHttpClientMock.Verify(t =>
                t.SendPostRequestAsync<CreateTransferTransactionRequest, List<Guid>>(It.IsAny<string>(),
                It.Is<CreateTransferTransactionRequest>(t => t.FromAccountId == fromAccountId && t.ToAccountId == toAccountId)),
                Times.Once);    
        }

        [Fact]
        public async Task CreateTransferTransactionAsync_ToAccountDeactivated_EntityConflictExceptionThrown()
        {
            //arrange
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            var message = $"Account with id {toAccountId} is deactivated.";
            var request = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, FromCurrency = Currency.EUR, ToCurrency = Currency.RUB };
            var fromAccount = new Account() { Id = fromAccountId, Currency = request.FromCurrency };
            var toAccount = new Account() { Id = toAccountId, Currency = request.ToCurrency, IsDeactivated = true };
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == fromAccountId)).ReturnsAsync(fromAccount);
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == toAccountId)).ReturnsAsync(toAccount);

            //Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateTransferTransactionAsync(request));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task CreateTransferTransactionAsync_AccountsOfDifferentCustomers_EntityConflictExceptionThrown()
        {
            //arrange
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            var message = "Accounts must belong to the same customer.";
            var request = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, FromCurrency = Currency.EUR, ToCurrency = Currency.RUB };
            var fromAccount = new Account() { Id = fromAccountId, Currency = request.FromCurrency, CustomerId = Guid.NewGuid() };
            var toAccount = new Account() { Id = toAccountId, Currency = request.ToCurrency, CustomerId = Guid.NewGuid() };
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == fromAccountId)).ReturnsAsync(fromAccount);
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == toAccountId)).ReturnsAsync(toAccount);

            //Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateTransferTransactionAsync(request));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task CreateTransferTransactionAsync_TransferFromDeactivatedAccountNotToRubAccount_EntityConflictExceptionThrown()
        {
            //arrange
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var message = $"Transfer from deactivated accounts is allowed only to the account with currency {Currency.RUB}.";
            var request = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, FromCurrency = Currency.JPY, ToCurrency = Currency.EUR };
            var fromAccount = new Account() { Id = fromAccountId, CustomerId = customerId, Currency = request.FromCurrency, IsDeactivated = true };
            var toAccount = new Account() { Id = toAccountId, CustomerId = customerId, Currency = request.ToCurrency };
            var customer = new Customer() { Id = customerId };
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == fromAccountId)).ReturnsAsync(fromAccount);
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == toAccountId)).ReturnsAsync(toAccount);
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);

            //Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateTransferTransactionAsync(request));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task CreateTransferTransactionAsync_CustomerDeactivated_EntityConflictExceptionThrown()
        {
            //arrange
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var message = $"Customer with id {customerId} is deactivated.";
            var request = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, FromCurrency = Currency.JPY, ToCurrency = Currency.EUR };
            var fromAccount = new Account() { Id = fromAccountId, CustomerId = customerId, Currency = request.FromCurrency };
            var toAccount = new Account() { Id = toAccountId, CustomerId = customerId, Currency = request.ToCurrency };
            var customer = new Customer() { Id = customerId, IsDeactivated = true };
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == fromAccountId)).ReturnsAsync(fromAccount);
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == toAccountId)).ReturnsAsync(toAccount);
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);

            //Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateTransferTransactionAsync(request));

            //Assert
            Assert.Equal(message, exception.Message);
        }
    }
}
