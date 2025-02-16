using AutoMapper;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;
using CustomersService.Presentation.Configuration;
using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers;

[ApiController]
[Route("api/accounts")]
[Authorize]
public class AccountController(
        IAccountService accountService,
        IMapper mapper,
        ILogger<AccountController> logger) 
    : ControllerBase
{
    [HttpPost]
    [CustomAuthorize([Role.Regular, Role.VIP])]
    public async Task<ActionResult<Guid>> CreateAsync([FromBody] AccountAddRequest request)
    {
        var accountToCreate = mapper.Map<AccountCreationModel>(request);
        var accountId = await accountService.CreateAsync(accountToCreate);
        return Ok(accountId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountFullInfoResponse>> GetByIdAsync([FromRoute] Guid id)
    {
        var userId = this.GetCustomerIdFromClaims();
        var account = await accountService.GetFullInfoByIdAsync(id, userId);
        var response = mapper.Map<AccountFullInfoResponse>(account);
        return Ok(response);
    }

    [HttpPatch("{id}/activate")]
    [CustomAuthorize([Role.Admin])]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id)
    {
        await accountService.ActivateAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    [CustomAuthorize([Role.Admin])]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id)
    {
        await accountService.DeactivateAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/transactions")]
    public async Task<ActionResult<List<TransactionResponse>>> GetTransactionsByAccountId([FromRoute] Guid id)
    {
        var customerId = this.GetCustomerIdFromClaims();
        var transactions = await accountService.GetTransactionsByAccountIdAsync(id, customerId);
        return Ok(transactions);
    }
}
