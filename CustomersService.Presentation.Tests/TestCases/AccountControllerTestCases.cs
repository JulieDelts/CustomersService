using CustomersService.Application.Models;
using CustomersService.Core.Enum;
using CustomersService.Presentation.Models.Requests;

namespace CustomersService.Presentation.Tests.TestCases;

internal static class AccountControllerTestCases
{
    public static IEnumerable<object[]> AccountToCreate()
    {
        var accountRequest = new AccountAddRequest()
        {
            CustomerId = Guid.NewGuid(),
            Currency = Currency.USD
        };

        yield return new object[] { accountRequest };
    }

    public static IEnumerable<object[]> AccountFullInfo()
    {
        var accountFullInfo = new AccountFullInfoModel()
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Currency = Currency.USD,
            IsDeactivated = false,
            Balance = 1000,
            DateCreated = DateTime.Now
        };

        yield return new object[] { accountFullInfo };
    }
}