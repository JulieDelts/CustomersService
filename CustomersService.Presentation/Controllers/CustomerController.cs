using AutoMapper;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Core.Enum;
using CustomersService.Presentation.Configuration;
using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomerController(
        ICustomerService customerService,
        IAccountService accountService,
        IMapper mapper,
        ILogger<CustomerController> logger
    ) : ControllerBase
{

    [HttpPost, AllowAnonymous]
    public async Task<ActionResult<Guid>> RegisterAsync([FromBody] RegisterCustomerRequest request)
    {
        var registrationModel = mapper.Map<CustomerRegistrationModel>(request);
        var id = await customerService.RegisterAsync(registrationModel);
        return Ok(id);
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequest request)
    {
        var token = await customerService.AuthenticateAsync(request.Email, request.Password);
        return Ok(token);
    }

    [HttpGet]
    [CustomAuthorize([Role.Admin])]
    public async Task<ActionResult<List<CustomerResponse>>> GetAllAsync([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var customerModels = await customerService.GetAllAsync(pageNumber, pageSize);
        var customers = mapper.Map<List<CustomerResponse>>(customerModels);
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerFullResponse>> GetByIdAsync([FromRoute] Guid id)
    {
        var customerModel = await customerService.GetFullInfoByIdAsync(id);
        var customer = mapper.Map<CustomerFullResponse>(customerModel);
        return Ok(customer);
    }


    [HttpGet("{id}/accounts")]
    public async Task<ActionResult<List<AccountResponse>>> GetAccountsByCustomerIdAsync([FromRoute] Guid id)
    {
        var customerId = this.GetCustomerIdFromClaims();
        var customerRole = this.GetCustomerRoleFromClaims();

        if (customerRole != Role.Admin && id != customerId)
            return Forbid();

        var accounts = await accountService.GetAllByCustomerIdAsync(id);
        var response = mapper.Map<List<AccountResponse>>(accounts);
        return Ok(response);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfileAsync([FromRoute] Guid id, [FromBody] CustomerUpdateRequest request)
    {
        var customerId = this.GetCustomerIdFromClaims();

        if (id != customerId)
            return Forbid();

        var customerUpdateModel = mapper.Map<CustomerUpdateModel>(request);
        await customerService.UpdateProfileAsync(id, customerUpdateModel);
        return NoContent();
    }

    [HttpPatch("{id}/password")]
    public async Task<IActionResult> UpdatePasswordAsync([FromRoute] Guid id, [FromBody] PasswordUpdateRequest request)
    {
        var customerId = this.GetCustomerIdFromClaims();

        if (id != customerId)
            return Forbid();

        await customerService.UpdatePasswordAsync(id, request.NewPassword, request.CurrentPassword);
        return NoContent();
    }

    [HttpPatch("{id}/vip")]
    [CustomAuthorize([Role.Admin])]
    public async Task<IActionResult> SetVipAsync([FromRoute] Guid id, [FromBody] SetVipRequest request)
    {
        await customerService.SetManualVipAsync(id, request.VipExpirationDate);
        return NoContent();
    }

    [HttpPatch("{id}/activate")]
    [CustomAuthorize([Role.Admin])]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id)
    {
        await customerService.ActivateAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    [CustomAuthorize([Role.Admin])]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id)
    {
        await customerService.DeactivateAsync(id);
        return NoContent();
    }
}
