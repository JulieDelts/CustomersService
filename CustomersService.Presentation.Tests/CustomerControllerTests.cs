
using System.Net;
using AutoMapper;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Presentation.Controllers;
using CustomersService.Presentation.Mappings;
using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using CustomersService.Presentation.Tests.TestCases;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CustomersService.Presentation.Tests
{
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly Mapper _mapper;
        private readonly CustomerController _sut;

        public CustomerControllerTests()
        {
            _customerServiceMock = new();
            var config = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile(new CustomerPresentationMapperProfile());
            });
            _mapper = new Mapper(config);
            _sut = new(_customerServiceMock.Object, _mapper);
        }

        [Fact]
        public async Task RegisterAsync_ValidModel_RegisterSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.OK;
            var customer = new RegisterCustomerRequest() { FirstName = "Edgar" };
            var customerModel = new CustomerRegistrationModel() { FirstName = "Edgar" };
            _customerServiceMock.Setup(t => t.RegisterAsync(customerModel)).ReturnsAsync(Guid.NewGuid());

            //Act
            var result = await _sut.RegisterAsync(customer);
            var statusCode = (result.Result as ObjectResult).StatusCode;

            //Assert
            Assert.IsType<ActionResult<Guid>>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _customerServiceMock.Verify(t =>
               t.RegisterAsync(It.Is<CustomerRegistrationModel>(t => t.FirstName == customerModel.FirstName)),
               Times.Once);
        }

        [Theory]
        [MemberData(nameof(CustomerControllerTestCases.CustomerToRegister), MemberType = typeof(CustomerControllerTestCases))]
        public void RegisterAsync_ValidModel_MappingSuccess(RegisterCustomerRequest customerRegisterRequest)
        {
            //Act
            var customerModel = _mapper.Map<CustomerRegistrationModel>(customerRegisterRequest);

            //Assert
            customerModel.Should().BeEquivalentTo(customerRegisterRequest);
        }

        [Fact]
        public async Task GetAllAsync_GetSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.OK;
            var pageNumber = 5;
            var pageSize = 10;
            var customers = new List<CustomerInfoModel>();
            _customerServiceMock.Setup(t => t.GetAllAsync(pageNumber, pageSize)).ReturnsAsync(customers);

            //Act
            var result = await _sut.GetAllAsync(pageNumber, pageSize);
            var statusCode = (result.Result as ObjectResult).StatusCode;

            //Assert
            Assert.IsType<ActionResult<List<CustomerResponse>>>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _customerServiceMock.Verify(t =>
               t.GetAllAsync(pageNumber, pageSize),
               Times.Once);
        }

        [Theory]
        [MemberData(nameof(CustomerControllerTestCases.Customers), MemberType = typeof(CustomerControllerTestCases))]
        public void GetAllAsync_ValidModel_MappingSuccess(List<CustomerInfoModel> customerModels)
        {
            //Act
            var customers = _mapper.Map<List<CustomerResponse>>(customerModels);

            //Assert
            customers.Should().BeEquivalentTo(customerModels);
        }

        [Fact]
        public async Task GetByIdAsync_GetSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.OK;
            var id = Guid.NewGuid();    
            var customerModel = new CustomerFullInfoModel();
            _customerServiceMock.Setup(t => t.GetFullInfoByIdAsync(id)).ReturnsAsync(customerModel);

            //Act
            var result = await _sut.GetByIdAsync(id);
            var statusCode = (result.Result as ObjectResult).StatusCode;

            //Assert
            Assert.IsType<ActionResult<CustomerFullResponse>>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _customerServiceMock.Verify(t =>
               t.GetFullInfoByIdAsync(id),
               Times.Once);
        }

        [Theory]
        [MemberData(nameof(CustomerControllerTestCases.CustomerFullInfo), MemberType = typeof(CustomerControllerTestCases))]
        public void GetByIdAsync_ValidModel_MappingSuccess(CustomerFullInfoModel customerModel)
        {
            //Act
            var customer = _mapper.Map<CustomerFullResponse>(customerModel);

            //Assert
            customer.Should().BeEquivalentTo(customerModel);
        }

        [Fact]
        public async Task UpdateProfileAsync_ValidModel_UpdateSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.NoContent;
            var id = Guid.NewGuid();
            var customerUpdate = new CustomerUpdateRequest() { FirstName = "Edgar" };

            //Act
            var result = await _sut.UpdateProfileAsync(id, customerUpdate);
            var statusCode = (result as NoContentResult).StatusCode;

            //Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _customerServiceMock.Verify(t =>
               t.UpdateProfileAsync(id, It.Is<CustomerUpdateModel>(t => t.FirstName == customerUpdate.FirstName)),
               Times.Once);
        }

        [Theory]
        [MemberData(nameof(CustomerControllerTestCases.CustomerUpdate), MemberType = typeof(CustomerControllerTestCases))]
        public void UpdateProfileAsync_ValidModel_MappingSuccess(CustomerUpdateRequest customerUpdateRequest)
        {
            //Act
            var customerUpdateModel = _mapper.Map<CustomerUpdateModel>(customerUpdateRequest);

            //Assert
            customerUpdateModel.Should().BeEquivalentTo(customerUpdateRequest);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ValidModel_UpdateSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.NoContent;
            var id = Guid.NewGuid();
            var passwordRequest = new PasswordUpdateRequest() { NewPassword = "NewTestPassword", CurrentPassword = "CurrentTestPassword" };

            //Act
            var result = await _sut.UpdatePasswordAsync(id, passwordRequest);
            var statusCode = (result as NoContentResult).StatusCode;

            //Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _customerServiceMock.Verify(t =>
               t.UpdatePasswordAsync(id, passwordRequest.NewPassword, passwordRequest.CurrentPassword),
               Times.Once);
        }

        [Fact]
        public async Task SetVipAsync_ValidModel_SetVipSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.NoContent;
            var id = Guid.NewGuid();
            var setVipRequest = new SetVipRequest() { VipExpirationDate = DateTime.Now };

            //Act
            var result = await _sut.SetVipAsync(id, setVipRequest);
            var statusCode = (result as NoContentResult).StatusCode;

            //Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _customerServiceMock.Verify(t =>
               t.SetManualVipAsync(id, setVipRequest.VipExpirationDate),
               Times.Once);
        }

        [Fact]
        public async Task ActivateAsync_ActivateSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.NoContent;
            var id = Guid.NewGuid();

            //Act
            var result = await _sut.ActivateAsync(id);
            var statusCode = (result as NoContentResult).StatusCode;

            //Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _customerServiceMock.Verify(t =>
               t.ActivateAsync(id),
               Times.Once);
        }

        [Fact]
        public async Task DeactivateAsync_ActivateSuccess()
        {
            // Arrange
            var expectedStatusCode = HttpStatusCode.NoContent;
            var id = Guid.NewGuid();

            //Act
            var result = await _sut.DeactivateAsync(id);
            var statusCode = (result as NoContentResult).StatusCode;

            //Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal((int)expectedStatusCode, statusCode);
            _customerServiceMock.Verify(t =>
               t.DeactivateAsync(id),
               Times.Once);
        }
    }
}
