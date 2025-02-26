using CustomersService.Application.Exceptions;
using System.Net;
using CustomersService.Application.Integrations;
using Microsoft.Extensions.Logging;
using Moq;
using CustomersService.Persistence.Entities;
using CustomersService.Core.IntegrationModels.Responses;
using System.Text.Json;
using FluentAssertions;
using CustomersService.Core.IntegrationModels.Requests;

namespace CustomersService.Application.Tests;

public class CommonHttpClientTests
{
    private Mock<HttpMessageHandler> _messageHandlerMock;
    private readonly Mock<ILogger<CommonHttpClient>> _commonHttpClientLoggerMock;
    private CommonHttpClient _sut;

    public CommonHttpClientTests()
    {
        _messageHandlerMock = new();
        _commonHttpClientLoggerMock = new();
        var httpClient = new HttpClient(_messageHandlerMock.Object) { BaseAddress =  new Uri("http://194.147.90.249:9091/api") };
        _sut = new(httpClient, _commonHttpClientLoggerMock.Object);
    }

    [Fact]
    public async Task SendGetRequestAsync_RequestSuccess()
    {
        //Arrange
        var accountId = Guid.NewGuid();
        var path = "accounts";
        var account = new Account() { Id = accountId };
        var accountBalance = new BalanceResponse()
        {
            AccountId = accountId,
            Balance = 10000
        };
        var response = JsonSerializer.Serialize(accountBalance);
        HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.OK, response);

        //act 
        var result = await _sut.SendGetRequestAsync<BalanceResponse>($"{path}/{accountId}/balance");

        // Assert
        result.Should().BeEquivalentTo(accountBalance);
    }

    [Fact]
    public async Task SendGetRequestAsync_InternalServerErrorReturned_BadGatewayExceptionThrown()
    {
        //arrange
        var accountId = Guid.NewGuid();
        var path = "accounts";
        var message = "Invalid response from the upstream server.";
        HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.InternalServerError);

        //act 
        var exception = await Assert.ThrowsAsync<BadGatewayException>(async () => await _sut.SendGetRequestAsync<BalanceResponse>($"{path}/{accountId}/balance"));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task SendGetRequestAsync_RequestErrorReturned_ServiceUnavailableExceptionThrown()
    {
        //arrange
        var accountId = Guid.NewGuid();
        var path = "accounts";
        var message = "Request to the service failed.";
        HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.NotFound);

        //act 
        var exception = await Assert.ThrowsAsync<ServiceUnavailableException>(async () => await _sut.SendGetRequestAsync<BalanceResponse>($"{path}/{accountId}/balance"));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task SendPostRequestAsync_RequestSuccess()
    {
        //Arrange
        var accountId = Guid.NewGuid();
        var path = "transactions";
        var request = new CreateTransactionRequest() { AccountId = accountId };
        var transactionId = Guid.NewGuid();
        var response = JsonSerializer.Serialize(transactionId);
        HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.OK, response);

        //act 
        var result = await _sut.SendPostRequestAsync<CreateTransactionRequest, Guid>($"{path}/deposit", request);

        // Assert
        Assert.Equal(transactionId, result);
    }

    [Fact]
    public async Task SendPostRequestAsync_InternalServerErrorReturned_BadGatewayExceptionThrown()
    {
        //arrange
        var accountId = Guid.NewGuid();
        var path = "transactions";
        var message = "Invalid response from the upstream server.";
        var request = new CreateTransactionRequest() { AccountId = accountId };
        HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.InternalServerError);

        //act 
        var exception = await Assert.ThrowsAsync<BadGatewayException>(async () => await _sut.SendPostRequestAsync<CreateTransactionRequest, Guid>($"{path}/deposit", request));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task SendPostRequestAsync_RequestErrorReturned_ServiceUnavailableExceptionThrown()
    {
        //arrange
        var accountId = Guid.NewGuid();
        var path = "transactions";
        var message = "Request to the service failed.";
        var request = new CreateTransactionRequest() { AccountId = accountId };
        HttpMessageHandlerMockSetup.SetupProtectedHttpMessageHandlerMock(_messageHandlerMock, HttpStatusCode.NotFound);

        //act 
        var exception = await Assert.ThrowsAsync<ServiceUnavailableException>(async () => await _sut.SendPostRequestAsync<CreateTransactionRequest, Guid>($"{path}/deposit", request));

        // Assert
        Assert.Equal(message, exception.Message);
    }
}
