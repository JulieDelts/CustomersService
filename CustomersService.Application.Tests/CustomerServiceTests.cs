
using AutoMapper;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Application.Services;
using CustomersService.Persistence.Interfaces;
using Moq;
using CustomersService.Application.Mappings;
using CustomersService.Application.Models;
using CustomersService.Persistence.Entities;
using CustomersService.Core.Enum;
using CustomersService.Application.Exceptions;
using CustomersService.Application.Tests.TestCases;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CustomersService.Core;

namespace CustomersService.Application.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ICustomerUnitOfWork> _customerUnitOfWorkMock;
        private readonly Mock<ILogger<CustomerUtils>> _customerUtilsLoggerMock;
        private readonly Mock<ILogger<CustomerService>> _customerServiceLoggerMock;
        private readonly Mock<IOptions<AuthConfigOptions>> _authConfigOptionsMock;

        private readonly Mapper _mapper;
        private readonly CustomerService _sut;

        public CustomerServiceTests()
        {
            _accountRepositoryMock = new();
            _customerRepositoryMock = new();
            _customerUnitOfWorkMock = new();
            _customerUtilsLoggerMock = new();
            _customerServiceLoggerMock = new();
            _authConfigOptionsMock = new();
            var config = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile(new CustomerApplicationMapperProfile());
            });
            _mapper = new Mapper(config);
            _sut = new(
                _customerRepositoryMock.Object,
                _accountRepositoryMock.Object,
                _mapper,
                new CustomerUtils(_customerRepositoryMock.Object,
                _customerUtilsLoggerMock.Object,
                _authConfigOptionsMock.Object),
                _customerUnitOfWorkMock.Object,
                _customerServiceLoggerMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_CustomerNotAlreadyRegistered_RegisterSuccess()
        {
            // Arrange
            var customer = new CustomerRegistrationModel()
            {
                Email = "TestLogin",
                Password = "TestPassword"
            };

            var account = new Account() { Currency = Currency.RUB };

            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Email == customer.Email)).ReturnsAsync(null as Customer);

            // Act
            await _sut.RegisterAsync(customer);

            //Assert
            _customerUnitOfWorkMock.Verify(t =>
                t.CreateCustomerAsync(It.Is<Customer>(t => t.Email == customer.Email), It.Is<Account>(t => t.Currency == account.Currency)),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterAsync_CustomerAlreadyRegistered_EntityConflictExceptionThrown()
        {
            var customer = new CustomerRegistrationModel()
            {
                Email = "TestLogin",
                Password = "TestPassword"
            };

            var message = $"Customer with email {customer.Email} already exists.";
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Email == customer.Email)).ReturnsAsync(new Customer() { Email = customer.Email });

            // Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.RegisterAsync(customer));

            // Assert
            Assert.Equal(message, exception.Message);
        }


        [Fact]
        public async Task RegisterAsync_TransactionFailed_TransactionFailedExceptionThrown()
        {
            // Arrange
            var message = "Transaction failed.";
            var customer = new CustomerRegistrationModel()
            {
                Email = "TestLogin",
                Password = "TestPassword"
            };
            var account = new Account() { Currency = Currency.RUB };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Email == customer.Email)).ReturnsAsync(null as Customer);
            _customerUnitOfWorkMock.Setup(t => t.CreateCustomerAsync(It.IsAny<Customer>(), It.IsAny<Account>())).ThrowsAsync(new Exception());

            //Act
            var exception = await Assert.ThrowsAsync<TransactionFailedException>(async () => await _sut.RegisterAsync(customer));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Theory]
        [MemberData(nameof(CustomerServiceTestCases.CustomerToRegister), MemberType = typeof(CustomerServiceTestCases))]
        public void RegisterAsync_ValidModel_MappingSuccess(CustomerRegistrationModel customerModel)
        {
            //Act 
            var customer = _mapper.Map<Customer>(customerModel);

            //Assert
            customer.Should().BeEquivalentTo(customerModel, options => options.ExcludingMissingMembers());
            Assert.Equal(customer.Id, Guid.Empty);
            Assert.False(customer.IsDeactivated);
            Assert.Empty(customer.Accounts);
            Assert.Equal(Role.Unknown, customer.Role);
            Assert.Null(customer.CustomVipDueDate);
        }

        [Fact]
        public async Task GetAllAsync_GetSuccess()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var pageNumber = 0;
            var pageSize = 5;

            // Act
            var result = await _sut.GetAllAsync(pageNumber, pageSize);

            // Assert
            _customerRepositoryMock.Verify(t =>
                t.GetAllAsync(pageNumber, pageSize),
                Times.Once
            );
        }

        [Theory]
        [MemberData(nameof(CustomerServiceTestCases.Customers), MemberType = typeof(CustomerServiceTestCases))]
        public void GetAllAsync_ValidModel_MappingSuccess(List<Customer> customerDTOs)
        {
            //Act
            var customers = _mapper.Map<List<CustomerInfoModel>>(customerDTOs);

            //Assert
            customers.Should().BeEquivalentTo(customerDTOs, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [MemberData(nameof(CustomerServiceTestCases.CustomerWithFullInfo), MemberType = typeof(CustomerServiceTestCases))]
        public void GetFullInfoByIdAsync_ValidModel_MappingSuccess(Customer customerDTO)
        {
            //Act 
            var customer = _mapper.Map<CustomerFullInfoModel>(customerDTO);

            //Assert
            customer.Should().BeEquivalentTo(customerDTO, options => options.ExcludingMissingMembers());
        }

        [Fact]
        public async Task UpdatePasswordAsync_ActiveCustomerValidPassword_UpdatePasswordSuccess()
        {
            //Arrange
            var customerId = Guid.NewGuid();
            var currentPassword = "CurrentPassword";
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(currentPassword);
            var newPassword = "NewPassword";
            var customer = new Customer() { Id = customerId, Password = hashedPassword };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync( c => c.Id == customerId)).ReturnsAsync(customer);
           
            //Act
            await _sut.UpdatePasswordAsync(customerId, newPassword, currentPassword);

            //Assert
            _customerRepositoryMock.Verify(t =>
               t.UpdatePasswordAsync(It.Is<Customer>(t => t.Id == customerId), It.IsAny<string>()),
               Times.Once
            );
        }

        [Fact]
        public async Task UpdatePasswordAsync_CustomerDeactivated_EntityConflictExceptionThrown()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var message = $"Customer with id {customerId} is deactivated.";
            var currentPassword = "CurrentPassword";
            var newPassword = "NewPassword";
            var customer = new Customer() { Id = customerId, IsDeactivated = true };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
           
            //Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => 
                await _sut.UpdatePasswordAsync(customerId, newPassword, currentPassword));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task UpdatePasswordAsync_WrongCurrentPassword_WrongCredentialsException()
        {
            //Arrange
            var customerId = Guid.NewGuid();
            var message = "The credentials are not correct.";
            var invalidCurrentPassword = "WrongCurrentPassword";
            var newPassword = "NewPassword";
            var validCurrentPassword = "ValidCurrentPassword";
            var validCurrentPasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(validCurrentPassword);
            var customer = new Customer() { Id = customerId, Password = validCurrentPasswordHash };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
      
            //Act
            var exception = await Assert.ThrowsAsync<WrongCredentialsException>(async () => await _sut.UpdatePasswordAsync(customerId, newPassword, invalidCurrentPassword));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task UpdateProfileAsync_ActiveCustomer_UpdateProfileSuccess()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer() { Id = customerId };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
            var customerUpdateModel = new CustomerUpdateModel() { FirstName = "FirstName", LastName = "LastName" };

            // Act
            await _sut.UpdateProfileAsync(customerId, customerUpdateModel);

            // Assert
            _customerRepositoryMock.Verify(t =>
                t.UpdateProfileAsync(It.Is<Customer>(t => t.Id == customerId),
                It.Is<Customer>(t => t.FirstName == customerUpdateModel.FirstName && t.LastName == customerUpdateModel.LastName)),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateProfileAsync_CustomerDeactivated_EntityConflictExceptionThrown()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var message = $"Customer with id {customerId} is deactivated.";
            var customer = new Customer() { Id = customerId, IsDeactivated = true };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
            var customerUpdateModel = new CustomerUpdateModel();

            //Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.UpdateProfileAsync(customerId, customerUpdateModel));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Theory]
        [MemberData(nameof(CustomerServiceTestCases.CustomerToUpdate), MemberType = typeof(CustomerServiceTestCases))]
        public void UpdateProfileAsync_ValidModel_MappingSuccess(CustomerUpdateModel customerUpdateModel)
        {
            //Act 
            var customer = _mapper.Map<Customer>(customerUpdateModel);

            //Assert
            customer.Should().BeEquivalentTo(customerUpdateModel, options => options.ExcludingMissingMembers());
            Assert.Equal(customer.Id, Guid.Empty);
            Assert.False(customer.IsDeactivated);
            Assert.Empty(customer.Accounts);
            Assert.Equal(Role.Unknown, customer.Role);
            Assert.Null(customer.CustomVipDueDate);
        }

        [Fact]
        public async Task DeactivateAsync_DeactivateSuccess()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer() { Id = customerId };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);

            // Act
            await _sut.DeactivateAsync(customerId);

            // Assert
            _customerRepositoryMock.Verify(t =>
                t.DeactivateAsync(It.Is<Customer>(t => t.Id == customerId)),
                Times.Once
            );
        }

        [Fact]
        public async Task ActivateAsync_ActivateSuccess()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer() { Id = customerId };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);

            // Act
            await _sut.ActivateAsync(customerId);

            // Assert
            _customerRepositoryMock.Verify(t =>
                t.ActivateAsync(It.Is<Customer>(t => t.Id == customerId)),
                Times.Once
            );
        }

        [Fact]
        public async Task SetManualVipAsync_SetManualVipSuccess()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var vipExpirationDate = DateTime.UtcNow;
            var regularAccounts = new List<Currency> { Currency.RUB, Currency.USD, Currency.EUR };
            var customer = new Customer() { Id = customerId };
            var accounts = new List<Account>() { new Account() { CustomerId = customerId, Currency = Currency.JPY }};
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
            _accountRepositoryMock.Setup(t => t.GetAllByConditionAsync(a =>
            a.CustomerId == customerId && !regularAccounts.Contains(a.Currency))).ReturnsAsync(accounts);

            // Act
            await _sut.SetManualVipAsync(customerId, vipExpirationDate);

            // Assert
            _customerUnitOfWorkMock.Verify(t =>
                t.SetManualVipAsync(It.Is<Customer>(t => t.Id == customerId), It.IsAny<DateTime>(), It.IsAny<List<Account>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task SetManualVipAsync_CustomerDeactivated_EntityConflictExceptionThrown()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var message = $"Customer with id {customerId} is deactivated.";
            var vipExpirationDate = DateTime.UtcNow;
            var customer = new Customer() { Id = customerId, IsDeactivated = true };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);

            //Act
            var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.SetManualVipAsync(customerId, vipExpirationDate));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task SetManualVipAsync_TransactionFailed_TransactionFailedExceptionThrown()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var message = "Transaction failed.";
            var vipExpirationDate = DateTime.UtcNow;
            var regularAccounts = new List<Currency> { Currency.RUB, Currency.USD, Currency.EUR };
            var customer = new Customer() { Id = customerId };
            var accounts = new List<Account>() { new Account() { Id = Guid.NewGuid(), CustomerId = customerId, Currency = Currency.JPY } };
            _customerRepositoryMock.Setup(t => t.GetByConditionAsync(c => c.Id == customerId)).ReturnsAsync(customer);
            _accountRepositoryMock.Setup(t => t.GetAllByConditionAsync(a => a.CustomerId == customerId && !regularAccounts.Contains(a.Currency))).ReturnsAsync(accounts);
            _customerUnitOfWorkMock.Setup(t => t.SetManualVipAsync(It.IsAny<Customer>(), It.IsAny<DateTime>(), It.IsAny<List<Account>>())).ThrowsAsync(new Exception());

            //Act
            var exception = await Assert.ThrowsAsync<TransactionFailedException>(async () => await _sut.SetManualVipAsync(customerId, vipExpirationDate));

            //Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public async Task BatchUpdateRoleAsync_BatchUpdateRoleSuccess()
        {
            // Arrange
            var customerIds = new List<Guid>();

            // Act
            await _sut.BatchUpdateRoleAsync(customerIds);

            // Assert
            _customerUnitOfWorkMock.Verify(t =>
                t.BatchUpdateRoleAsync(It.IsAny<List<Guid>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task BatchUpdateRoleAsync_TransactionFailed_TransactionFailedExceptionThrown()
        {
            // Arrange
            var customerIds = new List<Guid>();
            var message = "Transaction failed.";
            _customerUnitOfWorkMock.Setup(t => t.BatchUpdateRoleAsync(It.IsAny<List<Guid>>())).ThrowsAsync(new Exception());

            //Act
            var exception = await Assert.ThrowsAsync<TransactionFailedException>(async () => await _sut.BatchUpdateRoleAsync(customerIds));

            //Assert
            Assert.Equal(message, exception.Message);
        }
    }
}
