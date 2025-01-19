using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAsync([FromBody] AccountAddRequest request)
    {
        var last = new Guid();
        return Ok(last);
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountResponse>>> GetAllAsync()
    {
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountResponse>> GetByIdAsync([FromRoute] Guid id)
    {
        var customer = new AccountResponse();
        return Ok(customer);
    }

    [HttpPatch("{id}/activate)")]
    public async Task<IActionResult> ActivateAsync([FromRoute] Guid id)
    {
        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
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
