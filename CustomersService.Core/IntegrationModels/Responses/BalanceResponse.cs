﻿namespace CustomersService.Core.IntegrationModels.Responses;

public class BalanceResponse
{
    public Guid AccountId { get; set; }
    public decimal Balance { get; set; }
}
