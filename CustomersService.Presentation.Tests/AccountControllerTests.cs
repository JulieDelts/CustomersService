﻿using System.Net;
using AutoMapper;
using Castle.Core.Logging;
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
using Microsoft.Extensions.Logging;
using Moq;

namespace CustomersService.Presentation.Tests;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly Mapper _mapper;
    private readonly AccountController _sut;
    private readonly Mock<ILogger<AccountController>> _loggerMock;

    public AccountControllerTests()
    {
        _accountServiceMock = new();
        _loggerMock = new();
        var config = new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile(new AccountPresentationMapperProfile());
        });
        _mapper = new Mapper(config);
        _sut = new AccountController(_accountServiceMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidModel_CreateSuccess()
    {
        // Arrange
        var expectedStatusCode = HttpStatusCode.OK;
        var accountRequest = new AccountAddRequest() { CustomerId = Guid.NewGuid(), Currency = Currency.USD };
        var accountModel = new AccountCreationModel() { CustomerId = accountRequest.CustomerId, Currency = accountRequest.Currency };
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
    public async Task GetAccountsByCustomerIdAsync_GetSuccess()
    {
        // Arrange
        var expectedStatusCode = HttpStatusCode.OK;
        var customerId = Guid.NewGuid();
        var accounts = new List<AccountInfoModel>();
        _accountServiceMock.Setup(t => t.GetAllByCustomerIdAsync(customerId)).ReturnsAsync(accounts);

        //Act
        var result = await _sut.GetAccountsByCustomerIdAsync(customerId);
        var statusCode = (result.Result as ObjectResult).StatusCode;

        //Assert
        Assert.IsType<ActionResult<List<AccountResponse>>>(result);
        Assert.Equal((int)expectedStatusCode, statusCode);
        _accountServiceMock.Verify(t =>
           t.GetAllByCustomerIdAsync(customerId),
           Times.Once);
    }

    [Theory]
    [MemberData(nameof(AccountControllerTestCases.Accounts), MemberType = typeof(AccountControllerTestCases))]
    public void GetAccountsByCustomerIdAsync_ValidModel_MappingSuccess(List<AccountInfoModel> accountModels)
    {
        //Act
        var accounts = _mapper.Map<List<AccountResponse>>(accountModels);

        //Assert
        accounts.Should().BeEquivalentTo(accountModels);
    }

    [Fact]
    public async Task GetByIdAsync_GetSuccess()
    {
        // Arrange
        var expectedStatusCode = HttpStatusCode.OK;
        var id = Guid.NewGuid();
        var accountModel = new AccountFullInfoModel();
        _accountServiceMock.Setup(t => t.GetFullInfoByIdAsync(id)).ReturnsAsync(accountModel);

        //Act
        var result = await _sut.GetByIdAsync(id);
        var statusCode = (result.Result as ObjectResult).StatusCode;

        //Assert
        Assert.IsType<ActionResult<AccountFullInfoResponse>>(result);
        Assert.Equal((int)expectedStatusCode, statusCode);
        _accountServiceMock.Verify(t =>
           t.GetFullInfoByIdAsync(id),
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
    public async Task GetTransactionsByAccountId_GetSuccess()
    {
        // Arrange
        var expectedStatusCode = HttpStatusCode.OK;
        var id = Guid.NewGuid();
        var transactions = new List<TransactionResponse>();
        _accountServiceMock.Setup(t => t.GetTransactionsByAccountIdAsync(id)).ReturnsAsync(transactions);

        //Act
        var result = await _sut.GetTransactionsByAccountId(id);
        var statusCode = (result.Result as ObjectResult).StatusCode;

        //Assert
        Assert.IsType<ActionResult<List<TransactionResponse>>>(result);
        Assert.Equal((int)expectedStatusCode, statusCode);
        _accountServiceMock.Verify(t =>
           t.GetTransactionsByAccountIdAsync(id),
           Times.Once);
    }
}
