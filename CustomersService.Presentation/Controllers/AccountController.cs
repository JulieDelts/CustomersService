using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers;

[Route("api/Account")]
[ApiController]
public class AccountController : Controller
{
    [HttpPost]
    public ActionResult<Guid> AccountAdd([FromBody] AccountAddRequest request)
    {
        var last = new Guid();
        return Ok(last);
    }
    [HttpPut("{id}")]
    public IActionResult AccountUpdate([FromRoute] Guid id, [FromBody] AccountUpdateRequest request)
    {
        return NoContent();
    }
    [HttpGet("{id}")]
    public ActionResult<AccountResponse> GetAccount([FromRoute] Guid id)
    {
        var customer = new CustomerResponse();
        return Ok(customer);
    }
    [HttpGet]
    public ActionResult<List<AccountResponse>> GetAccounts()
    {

        return Ok();
    }
    [HttpDelete("{id}")]
    public ActionResult DeleteAccount([FromRoute] Guid id)
    {
        return NoContent();
    }
}
