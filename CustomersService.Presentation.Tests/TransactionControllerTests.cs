
using System.Net;
using AutoMapper;
using CustomersService.Application.Interfaces;
using CustomersService.Core.DTOs.Requests;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;
using CustomersService.Presentation.Controllers;
using CustomersService.Presentation.Mappings;
using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Tests.TestCases;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CustomersService.Presentation.Tests
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mapper _mapper;
        private readonly TransactionController _sut;

        public TransactionControllerTests()
        {
            _transactionServiceMock = new();
            var config = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile(new TransactionPresentationMapperProfile());
            });
            _mapper = new Mapper(config);
            _sut = new TransactionController(_transactionServiceMock.Object, _mapper);
        }

        [Fact]
        public async Task GetById_GetSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.OK;
            var id = Guid.NewGuid();
            var transaction = new TransactionResponse();
            _transactionServiceMock.Setup(t => t.GetByIdAsync(id)).ReturnsAsync(transaction);

            //Act
            var result = await _sut.GetByIdAsync(id);
            var statusCode = (result.Result as ObjectResult).StatusCode;

            //Assert
            Assert.IsType<ActionResult<TransactionResponse>>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _transactionServiceMock.Verify(t =>
               t.GetByIdAsync(id),
               Times.Once);
        }

        [Fact]
        public async Task CreateDepositTransactionAsync_ValidModel_GetSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.OK;
            var accountId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var transactionType = TransactionType.Deposit;
            var transaction = new CreateTransactionRequest() { AccountId = accountId, Amount = 100 };
            var transactionModel = new TransactionCreateRequest() { AccountId = accountId, Amount = 100 };
            UserClaimsMockSetup.SetUserClaims(_sut, customerId, Role.Regular);
            _transactionServiceMock.Setup(t => t.CreateSimpleTransactionAsync(transaction, customerId, transactionType)).ReturnsAsync(Guid.NewGuid());

            //Act
            var result = await _sut.CreateDepositTransactionAsync(transactionModel);
            var statusCode = (result.Result as ObjectResult).StatusCode;

            //Assert
            Assert.IsType<ActionResult<Guid>>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _transactionServiceMock.Verify(t =>
               t.CreateSimpleTransactionAsync(It.Is<CreateTransactionRequest>(t => 
               t.AccountId == transaction.AccountId && t.Amount == transaction.Amount), customerId, It.Is<TransactionType>(t => t == TransactionType.Deposit)),
               Times.Once);
        }

        [Fact]
        public async Task CreateWithdrawTransactionAsync_ValidModel_GetSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.OK;
            var accountId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var transactionType = TransactionType.Withdrawal;
            var transaction = new CreateTransactionRequest() { AccountId = accountId, Amount = 100 };
            var transactionModel = new TransactionCreateRequest() { AccountId = accountId, Amount = 100 };
            UserClaimsMockSetup.SetUserClaims(_sut, customerId, Role.Regular);
            _transactionServiceMock.Setup(t => t.CreateSimpleTransactionAsync(transaction, customerId, transactionType)).ReturnsAsync(Guid.NewGuid());

            //Act
            var result = await _sut.CreateWithdrawTransactionAsync(transactionModel);
            var statusCode = (result.Result as ObjectResult).StatusCode;

            //Assert
            Assert.IsType<ActionResult<Guid>>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _transactionServiceMock.Verify(t =>
               t.CreateSimpleTransactionAsync(It.Is<CreateTransactionRequest>(t =>
               t.AccountId == transaction.AccountId && t.Amount == transaction.Amount), customerId, It.Is<TransactionType>(t => t == TransactionType.Withdrawal)),
               Times.Once);
        } 
        
        [Theory]
        [MemberData(nameof(TransactionControllerTestCases.SimpleTransaction), MemberType = typeof(TransactionControllerTestCases))]
        public void CreateTransactionAsync_ValidModel_MappingSuccess(TransactionCreateRequest transactionCreateRequest)
        {
            //Act
            var transactionRequestModel = _mapper.Map<CreateTransactionRequest>(transactionCreateRequest);

            //Assert
            transactionCreateRequest.Should().BeEquivalentTo(transactionRequestModel);
        }

        [Fact]
        public async Task CreateTransferTransactionAsync_ValidModel_GetSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.OK;
            var customerId = Guid.NewGuid();
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            var transaction = new TransferTransactionCreateRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, Amount = 100 };
            var transactionModel = new CreateTransferTransactionRequest() { FromAccountId = fromAccountId, ToAccountId = toAccountId, Amount = 100 };
            UserClaimsMockSetup.SetUserClaims(_sut, customerId, Role.Regular);
            _transactionServiceMock.Setup(t => t.CreateTransferTransactionAsync(transactionModel, customerId)).ReturnsAsync(new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() });

            //Act
            var result = await _sut.CreateTransferTransactionAsync(transaction);
            var statusCode = (result.Result as ObjectResult).StatusCode;

            //Assert
            Assert.IsType<ActionResult<List<Guid>>>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _transactionServiceMock.Verify(t =>
               t.CreateTransferTransactionAsync(It.Is<CreateTransferTransactionRequest>(t => t.FromAccountId == transaction.FromAccountId 
               && t.ToAccountId == transaction.ToAccountId && t.Amount == transaction.Amount), customerId),
               Times.Once);
        }

        [Theory]
        [MemberData(nameof(TransactionControllerTestCases.TransferTransaction), MemberType = typeof(TransactionControllerTestCases))]
        public void CreateTransferTransactionAsync_ValidModel_MappingSuccess(TransferTransactionCreateRequest transactionCreateRequest)
        {
            //Act
            var transactionRequestModel = _mapper.Map<CreateTransferTransactionRequest>(transactionCreateRequest);

            //Assert
            transactionCreateRequest.Should().BeEquivalentTo(transactionRequestModel, options => options.ExcludingMissingMembers());
        }
    }
}
