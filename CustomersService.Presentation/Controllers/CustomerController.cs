using AutoMapper;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CustomersService.Presentation.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController(
        ICustomerService customerService, 
        IMapper mapper,
        ILogger<CustomerController> logger
    ) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<Guid>> RegisterAsync([FromBody] RegisterCustomerRequest request)
    {
        logger.LogInformation("Received request to register customer with email {Email}", request.Email);
        logger.LogTrace("Request data: {@Request}", request);

        var registrationModel = mapper.Map<CustomerRegistrationModel>(request);
        logger.LogTrace("Mapped CustomerRegistrationModel: {@RegistrationModel}", registrationModel);

        var id = await customerService.RegisterAsync(registrationModel);
        logger.LogInformation("Customer registered successfully with ID {CustomerId}", id);

        return Ok(id);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequest request)
    {
        logger.LogInformation("Received login request for email {Email}", request.Email);
        logger.LogTrace("Processing login request for email {Email}", request.Email);

        var token = string.Empty;

        logger.LogInformation("Login successful for email {Email}", request.Email);

        return Ok(token);
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerResponse>>> GetAllAsync([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        logger.LogInformation("Received request to get all customers with pageNumber {PageNumber} and pageSize {PageSize}", pageNumber, pageSize);
        logger.LogTrace("Calling customerService.GetAllAsync with pageNumber: {PageNumber}, pageSize: {PageSize}", pageNumber, pageSize);

        var customerModels = await customerService.GetAllAsync(pageNumber, pageSize);
        logger.LogTrace("Retrieved customer models: {@CustomerModels}", customerModels);

        var customers = mapper.Map<List<CustomerResponse>>(customerModels);
        logger.LogTrace("Mapped CustomerResponse: {@Customers}", customers);

        logger.LogInformation("Successfully retrieved {Count} customers", customers.Count);
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerFullResponse>> GetByIdAsync([FromRoute] Guid id)
    {
        logger.LogInformation("Received request to get customer details for customer {CustomerId}", id);
        logger.LogTrace("Calling customerService.GetFullInfoByIdAsync with CustomerId: {CustomerId}", id);

        var customerModel = await customerService.GetFullInfoByIdAsync(id);
        logger.LogTrace("Retrieved customer model: {@CustomerModel}", customerModel);

        var customer = mapper.Map<CustomerFullResponse>(customerModel);
        logger.LogTrace("Mapped CustomerFullResponse: {@Customer}", customer);

        logger.LogInformation("Successfully retrieved customer details for customer {CustomerId}", id);
        return Ok(customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfileAsync([FromRoute] Guid id, [FromBody] CustomerUpdateRequest request)
    {
        logger.LogInformation("Received request to update profile for customer {CustomerId}", id);
        logger.LogTrace("Request data: {@Request}", request);

        var customerUpdateModel = mapper.Map<CustomerUpdateModel>(request);
        logger.LogTrace("Mapped CustomerUpdateModel: {@CustomerUpdateModel}", customerUpdateModel);

        await customerService.UpdateProfileAsync(id, customerUpdateModel);
        logger.LogInformation("Successfully updated profile for customer {CustomerId}", id);

        return NoContent();
    }

    [HttpPatch("{id}/password")]
    public async Task<IActionResult> UpdatePasswordAsync([FromRoute] Guid id, [FromBody] PasswordUpdateRequest request)
    {
        logger.LogInformation("Received request to update password for customer {CustomerId}", id);
        logger.LogTrace("Request data: {@Request}", request);

        await customerService.UpdatePasswordAsync(id, request.NewPassword, request.CurrentPassword);
        logger.LogInformation("Successfully updated password for customer {CustomerId}", id);

        return NoContent();
    }

    [HttpPatch("{id}/vip")]
    public async Task<IActionResult> SetVipAsync([FromRoute] Guid id, [FromBody] SetVipRequest request)
    {
        logger.LogInformation("Received request to set VIP status for customer {CustomerId}", id);
        logger.LogTrace("Request data: {@Request}", request);

        await customerService.SetManualVipAsync(id, request.VipExpirationDate);
        logger.LogInformation("Successfully set VIP status for customer {CustomerId}", id);

        return NoContent();
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id)
    {
        logger.LogInformation("Received request to activate customer {CustomerId}", id);
        logger.LogTrace("Calling customerService.ActivateAsync with CustomerId: {CustomerId}", id);

        await customerService.ActivateAsync(id);
        logger.LogInformation("Successfully activated customer {CustomerId}", id);

        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id)
    {
        logger.LogInformation("Received request to deactivate customer {CustomerId}", id);
        logger.LogTrace("Calling customerService.DeactivateAsync with CustomerId: {CustomerId}", id);

        await customerService.DeactivateAsync(id);
        logger.LogInformation("Successfully deactivated customer {CustomerId}", id);

        return NoContent();
    }
}
