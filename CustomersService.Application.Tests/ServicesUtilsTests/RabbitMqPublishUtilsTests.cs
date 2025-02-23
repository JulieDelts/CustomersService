using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Persistence.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using MYPBackendMicroserviceIntegrations.Messages;

namespace CustomersService.Application.Tests.ServicesUtilsTests;

public class RabbitMqPublishUtilsTests
{
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<ILogger<RabbitMqPublishUtils>> _loggerMock;
    private readonly RabbitMqPublishUtils _sut;

    public RabbitMqPublishUtilsTests()
    {
        _publishEndpointMock = new();
        _loggerMock = new();

        _sut = new(_publishEndpointMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task PublishAccountUpdateAsync_PublishSuccess()
    {
        //Arrange
        var account = new Account() { Id = Guid.NewGuid() };

        //Act
        await _sut.PublishAccountUpdateAsync(account);

        //Assert
        _publishEndpointMock.Verify(t =>
           t.Publish(It.Is<AccountMessage>(t => t.Id == account.Id), It.IsAny<CancellationToken>()),
           Times.Once
        );
    }

    [Fact]
    public async Task PublishCustomerUpdateAsync_PublishSuccess()
    {
        //Arrange
        var customer = new Customer() { Id = Guid.NewGuid() };

        //Act
        await _sut.PublishCustomerUpdateAsync(customer);

        //Assert
        _publishEndpointMock.Verify(t =>
           t.Publish(It.Is<CustomerMessage>(t => t.Id == customer.Id), It.IsAny<CancellationToken>()),
           Times.Once
        );
    }

    [Fact]
    public async Task PublishRoleUpdateIds_PublishSuccess()
    {
        //Arrange
        var ids = new List<Guid>() { Guid.NewGuid() };

        //Act
        await _sut.PublishRoleUpdateIdsAsync(ids);

        //Assert
        _publishEndpointMock.Verify(t =>
           t.Publish(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
           Times.Once
        );
    }
}
