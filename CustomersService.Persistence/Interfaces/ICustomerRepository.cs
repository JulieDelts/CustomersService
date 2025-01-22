﻿using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces
{
    public interface ICustomerRepository: IBaseRepository<Customer>
    {
        Task UpdateProfileAsync(Customer customer, Customer customerUpdate);
        Task UpdatePasswordAsync(Customer customer, string newPassword);
    }
}
