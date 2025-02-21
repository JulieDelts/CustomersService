
using CustomersService.Application.Models;
using CustomersService.Core.Enum;
using CustomersService.Presentation.Models.Requests;

namespace CustomersService.Presentation.Tests.TestCases
{
    public static class CustomerControllerTestCases
    {
        public static IEnumerable<object[]> Customers()
        {
            var customerModels = new List<CustomerInfoModel>()
            {
                new CustomerInfoModel()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Phone = "89121234567",
                    Address = "TestAddress",
                    BirthDate = DateOnly.MinValue,
                    Id = Guid.NewGuid()
                },
                new CustomerInfoModel()
                {
                    FirstName = "Daisy",
                    LastName = "Foster",
                    Phone = "8922234567",
                    Address = "TestAddress",
                    BirthDate = DateOnly.MinValue,
                    Id = Guid.NewGuid()
                }
            };

            yield return new object[] { customerModels };
        }

        public static IEnumerable<object[]> Accounts()
        {
            var accountModels = new List<AccountInfoModel>()
                {
                new()
                {
                    Id = Guid.NewGuid(),
                    Currency = Currency.USD,
                    IsDeactivated = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Currency = Currency.EUR,
                    IsDeactivated = false
                }
            };

            yield return new object[] { accountModels };
        }

        public static IEnumerable<object[]> CustomerToRegister()
        {
            var customerRequest = new RegisterCustomerRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                Phone = "89121234567",
                Address = "TestAddress",
                BirthDate = DateOnly.MinValue,
                Email = "testemail",
                Password = "TestPassword"
            };

            yield return new object[] { customerRequest };
        }

        public static IEnumerable<object[]> CustomerFullInfo()
        {
            var customerFullInfo = new CustomerFullInfoModel()
            {
                FirstName = "John",
                LastName = "Doe",
                Phone = "89121234567",
                Address = "TestAddress",
                BirthDate = DateOnly.MinValue,
                Id = Guid.NewGuid(),
                IsDeactivated = false,
                Role = Role.VIP
            };

            yield return new object[] { customerFullInfo };
        }

        public static IEnumerable<object[]> CustomerUpdate()
        {
            var customerUpdate = new CustomerUpdateRequest()
            {
                FirstName = "Daisy",
                LastName = "Foster",
                Phone = "8922234567",
                Address = "TestAddress"
            };

            yield return new object[] { customerUpdate };
        }
    }
}
