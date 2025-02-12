using AutoMapper;
using CustomersService.Application.Models;
using CustomersService.Application.Services;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountController(
        AccountService accountService,
        IMapper mapper) 
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAsync([FromBody] AccountAddRequest request)
    {
        var accountToCreate = mapper.Map<AccountCreationModel>(request);
        var accountId = await accountService.CreateAsync(accountToCreate);
        return Ok(accountId);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<List<AccountResponse>>> GetAccountsByCustomerIdAsync([FromRoute] Guid customerId)
    {
        var accounts = await accountService.GetAllByCustomerIdAsync(customerId);
        var response = mapper.Map<List<AccountResponse>>(accounts);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountFullInfoResponse>> GetByIdAsync([FromRoute] Guid id)
    {
        var account = await accountService.GetFullInfoByIdAsync(id);
        var response = mapper.Map<AccountFullInfoResponse>(account);
        return Ok(response);
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id)
    {
        await accountService.ActivateAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id)
    {
        await accountService.DeactivateAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/transactions")]
    public async Task<ActionResult<List<TransactionResponse>>> GetTransactionsByAccountId([FromRoute] Guid id)
    {
        var transactions = await accountService.GetTransactionsByAccountIdAsync(id);
        return Ok(transactions);
    }
}
