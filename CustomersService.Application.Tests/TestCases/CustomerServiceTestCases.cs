
using CustomersService.Application.Models;
using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;

namespace CustomersService.Application.Tests.TestCases
{
    public static class CustomerServiceTestCases
    {
        public static IEnumerable<object[]> CustomerToRegister()
        {
            var customerModel = new CustomerRegistrationModel()
            {
                FirstName = "John",
                LastName = "Doe",
                Password = "Password",
                Email = "JohnDoeTest@gmail.com",
                Phone = "89121234567",
                Address = "TestAddress",
                BirthDate = DateOnly.MinValue
            };

            yield return new object[] { customerModel };
        }

        public static IEnumerable<object[]> Customers()
        {
            var customerDtos = new List<Customer>()
            {
               new Customer() { Id = Guid.NewGuid(), Phone = "TestPhone1", Address = "TestAddress1", 
                   BirthDate = DateOnly.MinValue, FirstName = "TestFirstName1", LastName = "TestLastName1" },
               new Customer() { Id = Guid.NewGuid(), Phone = "TestPhone2", Address = "TestAddress2", 
                   BirthDate = DateOnly.MinValue, FirstName = "TestFirstName2", LastName = "TestLastName2" }, 
               new Customer() { Id = Guid.NewGuid(), Phone = "TestPhone3", Address = "TestAddress3", 
                   BirthDate = DateOnly.MinValue, FirstName = "TestFirstName3", LastName = "TestLastName3" },
            };

            yield return new object[] { customerDtos };
        }

        public static IEnumerable<object[]> CustomerWithFullInfo()
        {
            var customerDto = new Customer()
            {
                Id = Guid.NewGuid(),
                Phone = "TestPhone1",
                Address = "TestAddress1",
                BirthDate = DateOnly.MinValue,
                FirstName = "TestFirstName1",
                LastName = "TestLastName1",
                IsDeactivated = false,
                Role = Role.Regular
            };

            yield return new object[] { customerDto };
        }

        public static IEnumerable<object[]> CustomerToUpdate()
        {
            var customerUpdateModel = new CustomerUpdateModel()
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "TestAddress",
                Phone = "89121234567"
            };

            yield return new object[] { customerUpdateModel };
        }
    }
}
