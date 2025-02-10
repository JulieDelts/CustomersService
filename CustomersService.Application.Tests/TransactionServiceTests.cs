
using System.Net;
using System.Text.Json;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Services;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;
using CustomersService.Persistence.Interfaces;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace CustomersService.Application.Tests
{
    public class TransactionServiceTests
    {
        private ITransactionService _sut;
        private Mock<HttpMessageHandler> _messageHandlerMock;
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly string _baseAddress = "http://194.147.90.249:9091/api/v1/transactions";

        public TransactionServiceTests()
        {
            _messageHandlerMock = new();
            _accountRepositoryMock = new();
            _customerRepositoryMock = new();
            _sut = new TransactionService(
                new CustomerUtils(_customerRepositoryMock.Object),
                new AccountUtils(_accountRepositoryMock.Object),
                _messageHandlerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_GetSuccess()
        {
            //arrange
            var id = Guid.NewGuid();
            var apiEndpoint = $"/{id}";
            var transaction = new TransactionResponse()
            {
                Id = id,
                AccountId = Guid.NewGuid(),
                Amount = 100,
                Date = DateTime.Now,
                TransactionType = TransactionType.Deposit
            };

            var response = JsonSerializer.Serialize(transaction); 
            
            var mockProtected = _messageHandlerMock.Protected();
            var setupApiRequest = mockProtected
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(m => m.RequestUri!.Equals(_baseAddress + apiEndpoint)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response)
                });

            //act 
            var result = await _sut.GetByIdAsync(id);

            //assert
           transaction.Should().BeEquivalentTo(result, options => options.ExcludingMissingMembers());

        }
    }
}
