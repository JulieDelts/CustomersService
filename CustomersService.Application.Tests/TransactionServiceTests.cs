using CustomersService.Application.Exceptions;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Services;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core;
using CustomersService.Core.IntegrationModels.Requests;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Application.Tests;

public class TransactionServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<ILogger<CustomerUtils>> _customerUtilsLoggerMock;
    private readonly Mock<ILogger<AccountUtils>> _accountUtilsLoggerMock;
    private readonly Mock<ILogger<TransactionService>> _transactionServiceLoggerMock;
    private readonly Mock<ICommonHttpClient> _commonHttpClientMock;
    private TransactionService _sut;

    public TransactionServiceTests()
    {
        _accountRepositoryMock = new();
        _customerRepositoryMock = new();
        _customerUtilsLoggerMock = new Mock<ILogger<CustomerUtils>>();
        _accountUtilsLoggerMock = new Mock<ILogger<AccountUtils>>();
        _transactionServiceLoggerMock = new Mock<ILogger<TransactionService>>();
        _commonHttpClientMock = new();
        _sut = new TransactionService(
            new CustomerUtils(_customerRepositoryMock.Object, _customerUtilsLoggerMock.Object, null),
            new AccountUtils(_accountRepositoryMock.Object, _accountUtilsLoggerMock.Object),
            _transactionServiceLoggerMock.Object,
            _commonHttpClientMock.Object,
            new Mock<IOptions<TransactionStoreApiConnectionStrings>>().Object);
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
        var account = new Account() { Id = accountId, CustomerId = customerId, Currency = Currency.USD };
        var customer = new Customer();
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);
        _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
        _commonHttpClientMock.Setup(t => t.SendPostRequestAsync<CreateTransactionRequest, Guid>($"{null}/deposit", request)).ReturnsAsync(transactionId);

        //Act 
        var result = await _sut.CreateSimpleTransactionAsync(request, customerId, transactionType);

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
        var customerId = Guid.NewGuid();
        var message = "TransactionType is not correct.";
        var transactionType = TransactionType.Transfer;
        var request = new CreateTransactionRequest() { AccountId = accountId };

        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateSimpleTransactionAsync(request, customerId, transactionType));

        //Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task CreateSimpleTransactionAsync_AccountDeactivated_EntityConflictExceptionThrown()
    {

        //Arrange
        var accountId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var message = $"Account with id {accountId} is deactivated.";
        var transactionType = TransactionType.Deposit;
        var account = new Account() { CustomerId = customerId, Id = accountId, IsDeactivated = true };
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);
        var request = new CreateTransactionRequest() { AccountId = accountId };

        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateSimpleTransactionAsync(request, customerId, transactionType));

        //Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task CreateSimpleTransactionAsync_WrongCurrency_EntityConflictExceptionThrown()
    {

        //Arrange
        var accountId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var transactionType = TransactionType.Deposit;
        var message = $"Deposit and withdraw transactions are only allowed for accounts with currencies {Currency.RUB}, {Currency.USD}.";
        var account = new Account() { Id = accountId, CustomerId = customerId, Currency = Currency.JPY };
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);
        var request = new CreateTransactionRequest() { AccountId = accountId };

        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateSimpleTransactionAsync(request, customerId, transactionType));

        //Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task CreateSimpleTransactionAsync_CustomerNotAccountOwner_AuthorizationFailedExceptionThrown()
    {

        //Arrange
        var accountId = Guid.NewGuid();
        var customerClaimId = Guid.NewGuid();   
        var customerId = Guid.NewGuid();
        var transactionType = TransactionType.Deposit;
        var message = "Customers are only allowed to create transactions for their own accounts.";
        var account = new Account() { Id = accountId, Currency = Currency.USD, CustomerId = customerId };
        var customer = new Customer() { Id = customerId };
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);
        _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
        var request = new CreateTransactionRequest() { AccountId = accountId };

        //Act
        var exception = await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await _sut.CreateSimpleTransactionAsync(request, customerClaimId, transactionType));

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
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateSimpleTransactionAsync(request, customerId, transactionType));

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
        var fromAccount = new Account() { Id = fromAccountId, CustomerId = customerId, Currency = Currency.USD };
        var toAccount = new Account() { Id = toAccountId, CustomerId = customerId, Currency = Currency.RUB };
        var request = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, FromCurrency = fromAccount.Currency, ToCurrency = toAccount.Currency };
        var customer = new Customer() { Id = customerId };
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == fromAccountId)).ReturnsAsync(fromAccount);
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == toAccountId)).ReturnsAsync(toAccount);
        _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
        _commonHttpClientMock.Setup(t => t.SendPostRequestAsync<CreateTransferTransactionRequest, List<Guid>>($"{null}/transfer", request)).ReturnsAsync(idList);

        //act 
        var result = await _sut.CreateTransferTransactionAsync(request, customerId);

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
        var customerId = Guid.NewGuid();
        var message = $"Account with id {toAccountId} is deactivated.";
        var fromAccount = new Account() { Id = fromAccountId, Currency = Currency.EUR };
        var toAccount = new Account() { Id = toAccountId, Currency = Currency.RUB, IsDeactivated = true };
        var request = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, FromCurrency = fromAccount.Currency, ToCurrency = toAccount.Currency };
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == fromAccountId)).ReturnsAsync(fromAccount);
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == toAccountId)).ReturnsAsync(toAccount);

        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateTransferTransactionAsync(request, customerId));

        //Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task CreateTransferTransactionAsync_AccountsOfDifferentCustomers_EntityConflictExceptionThrown()
    {
        //arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var message = "Accounts must belong to the same customer.";
        var fromAccount = new Account() { Id = fromAccountId, Currency = Currency.EUR, CustomerId = Guid.NewGuid() };
        var toAccount = new Account() { Id = toAccountId, Currency = Currency.RUB, CustomerId = Guid.NewGuid() };
        var request = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, FromCurrency = fromAccount.Currency, ToCurrency = toAccount.Currency };
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == fromAccountId)).ReturnsAsync(fromAccount);
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == toAccountId)).ReturnsAsync(toAccount);

        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateTransferTransactionAsync(request, customerId));

        //Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task CreateTransferTransactionAsync_CustomerNotAccountOwner_AuthorizationFailedExceptionThrown()
    {
        //arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var customerClaimId = Guid.NewGuid();
        var message = "Customers are only allowed to create transactions for their own accounts.";
        var fromAccount = new Account() { Id = fromAccountId, Currency = Currency.EUR, CustomerId = customerId };
        var toAccount = new Account() { Id = toAccountId, Currency = Currency.RUB, CustomerId = customerId };
        var request = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, FromCurrency = fromAccount.Currency, ToCurrency = toAccount.Currency };
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == fromAccountId)).ReturnsAsync(fromAccount);
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == toAccountId)).ReturnsAsync(toAccount);

        //Act
        var exception = await Assert.ThrowsAsync<AuthorizationFailedException>(async () => await _sut.CreateTransferTransactionAsync(request, customerClaimId));

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
        var fromAccount = new Account() { Id = fromAccountId, CustomerId = customerId, Currency = Currency.JPY, IsDeactivated = true };
        var toAccount = new Account() { Id = toAccountId, CustomerId = customerId, Currency = Currency.EUR };
        var request = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, FromCurrency = fromAccount.Currency, ToCurrency = toAccount.Currency };
        var customer = new Customer() { Id = customerId };
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == fromAccountId)).ReturnsAsync(fromAccount);
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == toAccountId)).ReturnsAsync(toAccount);
        _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);

        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateTransferTransactionAsync(request, customerId));

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
        var fromAccount = new Account() { Id = fromAccountId, CustomerId = customerId, Currency = Currency.EUR };
        var toAccount = new Account() { Id = toAccountId, CustomerId = customerId, Currency = Currency.RUB };
        var request = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, FromCurrency = fromAccount.Currency, ToCurrency = toAccount.Currency };
        var customer = new Customer() { Id = customerId, IsDeactivated = true };
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == fromAccountId)).ReturnsAsync(fromAccount);
        _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == toAccountId)).ReturnsAsync(toAccount);
        _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);

        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateTransferTransactionAsync(request, customerId));

        //Assert
        Assert.Equal(message, exception.Message);
    }
}
