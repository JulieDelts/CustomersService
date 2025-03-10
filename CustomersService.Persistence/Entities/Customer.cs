﻿using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Persistence.Entities;

public class Customer : BaseEntity
{
    public Role Role { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateOnly BirthDate { get; set; }
    public DateTime? CustomVipDueDate { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Account> Accounts { get; set; } = [];
}
