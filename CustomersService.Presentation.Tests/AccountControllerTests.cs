using System.Net;
using AutoMapper;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;
using CustomersService.Presentation.Controllers;
using CustomersService.Presentation.Mappings;
using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using CustomersService.Presentation.Tests.TestCases;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CustomersService.Presentation.Tests;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly Mapper _mapper;
    private readonly AccountController _sut;

    public AccountControllerTests()
    {
        _accountServiceMock = new();
        var config = new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile(new AccountPresentationMapperProfile());
        });
        _mapper = new Mapper(config);
        _sut = new AccountController(_accountServiceMock.Object, _mapper);
    }

    [Fact]
    public async Task CreateAsync_ValidModel_CreateSuccess()
    {
        // Arrange
        var expectedStatusCode = HttpStatusCode.OK;
        var customerId = Guid.NewGuid();
        var accountRequest = new AccountAddRequest() { CustomerId = customerId, Currency = Currency.USD };
        var accountModel = new AccountCreationModel() { CustomerId = accountRequest.CustomerId, Currency = accountRequest.Currency };
        UserClaimsMockSetup.SetUserClaims(_sut, customerId, Role.Regular);
        _accountServiceMock.Setup(t => t.CreateAsync(accountModel)).ReturnsAsync(Guid.NewGuid());

        //Act
        var result = await _sut.CreateAsync(accountRequest);
        var statusCode = (result.Result as ObjectResult).StatusCode;

        //Assert
        Assert.IsType<ActionResult<Guid>>(result);
        Assert.Equal((int)expectedStatusCode, statusCode);
        _accountServiceMock.Verify(t =>
           t.CreateAsync(It.Is<AccountCreationModel>(t => t.CustomerId == accountModel.CustomerId && t.Currency == accountModel.Currency)),
           Times.Once);
    }

    [Theory]
    [MemberData(nameof(AccountControllerTestCases.AccountToCreate), MemberType = typeof(AccountControllerTestCases))]
    public void CreateAsync_ValidModel_MappingSuccess(AccountAddRequest accountAddRequest)
    {
        //Act
        var accountModel = _mapper.Map<AccountCreationModel>(accountAddRequest);

        //Assert
        accountModel.Should().BeEquivalentTo(accountAddRequest);
    }

    [Fact]
    public async Task GetByIdAsync_GetSuccess()
    {
        // Arrange
        var expectedStatusCode = HttpStatusCode.OK;
        var id = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var accountModel = new AccountFullInfoModel();
        _accountServiceMock.Setup(t => t.GetFullInfoByIdAsync(id, customerId)).ReturnsAsync(accountModel);
        UserClaimsMockSetup.SetUserClaims(_sut, customerId, Role.Regular);

        //Act
        var result = await _sut.GetByIdAsync(id);
        var statusCode = (result.Result as ObjectResult).StatusCode;

        //Assert
        Assert.IsType<ActionResult<AccountFullInfoResponse>>(result);
        Assert.Equal((int)expectedStatusCode, statusCode);
        _accountServiceMock.Verify(t =>
           t.GetFullInfoByIdAsync(id, customerId),
           Times.Once);
    }

    [Theory]
    [MemberData(nameof(AccountControllerTestCases.AccountFullInfo), MemberType = typeof(AccountControllerTestCases))]
    public void GetByIdAsync_ValidModel_MappingSuccess(AccountFullInfoModel accountModel)
    {
        //Act
        var account = _mapper.Map<AccountFullInfoResponse>(accountModel);

        //Assert
        account.Should().BeEquivalentTo(accountModel);
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
        _accountServiceMock.Verify(t =>
           t.ActivateAsync(id),
           Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_DeactivateSuccess()
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
        _accountServiceMock.Verify(t =>
           t.DeactivateAsync(id),
           Times.Once);
    }

    [Fact]
    public async Task GetTransactionsByAccountIdAsyncs_GetSuccess()
    {
        // Arrange
        var expectedStatusCode = HttpStatusCode.OK;
        var id = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var transactions = new List<TransactionResponse>();
        UserClaimsMockSetup.SetUserClaims(_sut, customerId, Role.Regular);
        _accountServiceMock.Setup(t => t.GetTransactionsByAccountIdAsync(id, customerId)).ReturnsAsync(transactions);

        //Act
        var result = await _sut.GetTransactionsByAccountIdAsync(id);
        var statusCode = (result.Result as ObjectResult).StatusCode;

        //Assert
        Assert.IsType<ActionResult<List<TransactionResponse>>>(result);
        Assert.Equal((int)expectedStatusCode, statusCode);
        _accountServiceMock.Verify(t =>
           t.GetTransactionsByAccountIdAsync(id, customerId),
           Times.Once);
    }
}
