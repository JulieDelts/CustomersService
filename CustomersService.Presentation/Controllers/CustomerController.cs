using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> RegisterAsync([FromBody] RegisterCustomerRequest request)
    {
        var last = new Guid();
        return Ok(last);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginRequest request)
    {
        var token = string.Empty;
        return Ok(token);
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerResponse>>> GetAllAsync()
    {
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponse>> GetByIdAsync([FromRoute] Guid id)
    {
        var user = new CustomerResponse();
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfileAsync([FromRoute] Guid id, [FromBody] CustomerUpdateRequest request)
    {
        return NoContent();
    }

    [HttpPatch("{id}/password")]
    public async Task<IActionResult> UpdatePasswordAsync([FromRoute] Guid id, [FromBody] PasswordUpdateRequest request)
    {
        return NoContent();
    }

    [HttpPatch("{id}/activate)")]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id)
    {
        return NoContent();
    }

    [HttpPatch("{id}/deactivate)")]
    public async Task<IActionResult> DeactivateAsync([FromRoute] Guid id)
    {
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        return NoContent();
    }
}
