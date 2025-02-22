using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MYPBackendMicroserviceIntegrations.Enums;
using System.Security.Claims;

namespace CustomersService.Presentation.Tests;

public static class UserClaimsMockSetup
{
    public static void SetUserClaims(ControllerBase controller, Guid userId, Role userRole)
    {
        var claims = new List<Claim>
        {
            new Claim("SystemId", userId.ToString()),
            new Claim(ClaimTypes.Role, userRole.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
}
