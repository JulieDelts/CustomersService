using AutoMapper;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountController(
        IAccountService accountService,
        IMapper mapper,
        ILogger logger) 
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAsync([FromBody] AccountAddRequest request)
    {
        logger.LogInformation("Received request to create account for customer {CustomerId}", request.CustomerId);
        logger.LogDebug("Request data: {@Request}", request);

        var accountToCreate = mapper.Map<AccountCreationModel>(request);
        logger.LogDebug("Mapped AccountCreationModel: {@AccountToCreate}", accountToCreate);

        var accountId = await accountService.CreateAsync(accountToCreate);
        logger.LogInformation("Account created successfully with ID {AccountId}", accountId);

        return Ok(accountId);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<List<AccountResponse>>> GetAccountsByCustomerIdAsync([FromRoute] Guid customerId)
    {
        logger.LogInformation("Received request to get accounts for customer {CustomerId}", customerId);

        var accounts = await accountService.GetAllByCustomerIdAsync(customerId);
        logger.LogDebug("Retrieved accounts: {@Accounts}", accounts);

        var response = mapper.Map<List<AccountResponse>>(accounts);
        logger.LogDebug("Mapped AccountResponse: {@Response}", response);

        logger.LogInformation("Successfully retrieved {Count} accounts for customer {CustomerId}", response.Count, customerId);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountFullInfoResponse>> GetByIdAsync([FromRoute] Guid id)
    {
        logger.LogInformation("Received request to get account details for account {AccountId}", id);

        var account = await accountService.GetFullInfoByIdAsync(id);
        logger.LogDebug("Retrieved account: {@Account}", account);

        var response = mapper.Map<AccountFullInfoResponse>(account);
        logger.LogDebug("Mapped AccountFullInfoResponse: {@Response}", response);

        logger.LogInformation("Successfully retrieved account details for account {AccountId}", id);
        return Ok(response);
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id)
    {
        logger.LogInformation("Received request to activate account {AccountId}", id);

        await accountService.ActivateAsync(id);

        logger.LogInformation("Successfully activated account {AccountId}", id);
        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id)
    {
        logger.LogInformation("Received request to deactivate account {AccountId}", id);

        await accountService.DeactivateAsync(id);

        logger.LogInformation("Successfully deactivated account {AccountId}", id);
        return NoContent();
    }

    [HttpGet("{id}/transactions")]
    public async Task<ActionResult<List<TransactionResponse>>> GetTransactionsByAccountId([FromRoute] Guid id)
    {
        logger.LogInformation("Received request to get transactions for account {AccountId}", id);

        var transactions = await accountService.GetTransactionsByAccountIdAsync(id);
        logger.LogDebug("Retrieved transactions: {@Transactions}", transactions);

        logger.LogInformation("Successfully retrieved {Count} transactions for account {AccountId}", transactions.Count, id);
        return Ok(transactions);
    }
}
