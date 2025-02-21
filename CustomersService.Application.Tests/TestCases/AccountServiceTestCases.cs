
using CustomersService.Application.Models;
using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;

namespace CustomersService.Application.Tests.TestCases
{
    public static class AccountServiceTestCases
    {
        public static IEnumerable<object[]> AccountToCreate()
        {
            var accountModel = new AccountCreationModel()
            {
                Currency = Currency.USD,
                CustomerId = Guid.NewGuid()
            };

            yield return new object[] { accountModel };
        }

        public static IEnumerable<object[]> CustomerAccounts()
        {
            var customerId = Guid.NewGuid();

            var accountDtos = new List<Account>()
            {
                new Account { CustomerId = customerId, Currency = Currency.RUB },
                new Account { CustomerId = customerId, Currency = Currency.USD },
                new Account { CustomerId = customerId, Currency = Currency.JPY },
               
            };

            yield return new object[] { accountDtos };
        }

        public static IEnumerable<object[]> AccountWithFullInfo()
        {
            var accountDto = new Account()
            {
                Id = Guid.NewGuid(),
                Currency = Currency.USD,
                CustomerId = Guid.NewGuid(),
                DateCreated = DateTime.Now,
                IsDeactivated = false
            };

            yield return new object[] { accountDto };
        }
    }
}
