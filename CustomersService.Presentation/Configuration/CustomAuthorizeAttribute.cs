using Microsoft.AspNetCore.Authorization;
using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Presentation.Configuration;

public class CustomAuthorizeAttribute: AuthorizeAttribute
{
    public CustomAuthorizeAttribute(Role[] roles)
    {
        Roles = string.Join(",", roles);
    }
}
