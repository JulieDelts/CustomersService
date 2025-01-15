using CustomersService.Presentation.Models.Requests;
using CustomersService.Presentation.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomersService.Presentation.Controllers;

public class CustomerController : Controller
{
    [HttpPost]
    public ActionResult<Guid> Register([FromBody] RegisterCustomerRequest request)
    {
        var last = new Guid();
        return Ok(last);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        return Ok();
    }
    [HttpGet("{id}")]
    public ActionResult<CustomerWithResponse> GetCustomerById([FromRoute] Guid id)
    {
        var user = new CustomerWithResponse();
        return Ok(user);
    }
    [HttpGet]
    public ActionResult<List<CustomerResponse>> GetCustomers()
    {

        return Ok();
    }
    [HttpPut("{id}")]
    public IActionResult UpdateCustomer([FromRoute] Guid id, [FromBody] CustomerUpdateRequest request)
    {
        return NoContent();
    }
    [HttpDelete("{id}")]
    public ActionResult DeleteCustomer([FromRoute] Guid id)
    {
        return NoContent();
    }
    [HttpPatch("{id}/Deactivate)")]
    public IActionResult DiactivateCustomer([FromRoute] Guid id)
    {
        return NoContent();
    }
}
