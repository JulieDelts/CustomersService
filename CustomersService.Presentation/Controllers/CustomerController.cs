using AutoMapper;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController(ICustomerService customerService, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> RegisterAsync([FromBody] RegisterCustomerRequest request)
    {
        var registrationModel = mapper.Map<CustomerRegistrationModel>(request);
        
        var id = await customerService.RegisterAsync(registrationModel);

        return Ok(id);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequest request)
    {
        var token = string.Empty;
        return Ok(token);
    }

    [HttpGet]
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfileAsync([FromRoute] Guid id, [FromBody] CustomerUpdateRequest request)
    {
        var customerUpdateModel = mapper.Map<CustomerUpdateModel>(request);

        await customerService.UpdateProfileAsync(id,customerUpdateModel);

        return NoContent();
    }

    [HttpPatch("{id}/password")]
    public async Task<IActionResult> UpdatePasswordAsync([FromRoute] Guid id, [FromBody] PasswordUpdateRequest request)
    {
        await customerService.UpdatePasswordAsync(id, request.NewPassword, request.CurrentPassword);

        return NoContent();
    }

    [HttpPatch("{id}/vip")]
    public async Task<IActionResult> SetVipAsync([FromRoute] Guid id, [FromBody] SetVipRequest request)
    {
        await customerService.SetManualVipAsync(id, request.VipExpirationDate);

        return NoContent();
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id)
    {
        await customerService.ActivateAsync(id);

        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id)
    {
        await customerService.DeactivateAsync(id);

        return NoContent();
    }
}
