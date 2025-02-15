
using CustomersService.Application.Exceptions;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CustomersService.Application.Tests.ServicesUtilsTests
{
    public class AccountUtilsTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ILogger<AccountUtils>> _loggerMock;
        private readonly AccountUtils _sut;

        public AccountUtilsTests()
        {
            _accountRepositoryMock = new();
            _loggerMock = new();
            _sut = new(_accountRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingAccount_GetSuccess()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var account = new Account() { Id = accountId };
            _accountRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == accountId)).ReturnsAsync(account);

            // Act
            await _sut.GetByIdAsync(accountId);

            // Assert
            _accountRepositoryMock.Verify(t =>
                t.GetByConditionAsync(c => c.Id == accountId),
                Times.Once
            );
        }

        [Fact]
        public async Task GetByIdAsync_NotExistingAccount_EntityNotFoundExceptionThrown()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var message = $"Account with id {accountId} was not found.";

            // Act
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.GetByIdAsync(accountId));

            // Assert
            Assert.Equal(message, exception.Message);
        }
    }
}
