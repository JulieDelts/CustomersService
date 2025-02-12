
using Castle.Core.Logging;
using CustomersService.Application.Exceptions;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CustomersService.Application.Tests.ServicesUtilsTests
{
    public class CustomerUtilsTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ILogger<CustomerUtils>> _loggerMock;
        private readonly CustomerUtils _sut;

        public CustomerUtilsTests()
        {
            _customerRepositoryMock = new();
            _loggerMock = new();
            _sut = new(_customerRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingCustomer_GetSuccess()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer() { Id = customerId };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);

            // Act
            await _sut.GetByIdAsync(customerId);

            // Assert
            _customerRepositoryMock.Verify(t =>
                t.GetByConditionAsync(c => c.Id == customerId),
                Times.Once
            );
        }

        [Fact]
        public async Task GetByIdAsync_NotExistingCustomer_EntityNotFoundExceptionThrown()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var message = $"Customer with id {customerId} was not found.";

            // Act
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.GetByIdAsync(customerId));

            // Assert
            Assert.Equal(message, exception.Message);
        }
    }
}
