﻿using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Presentation.Models.Responses;

public class AccountFullInfoResponse
{
    public Guid Id { get; set; }
    public bool IsDeactivated { get; set; }
    public decimal Balance { get; set; }
    public DateTime DateCreated { get; set; }
    public Currency Currency { get; set; }
    public Guid CustomerId { get; set; }

}
