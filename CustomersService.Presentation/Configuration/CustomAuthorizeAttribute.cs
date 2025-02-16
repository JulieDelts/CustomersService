using CustomersService.Core.Enum;
using Microsoft.AspNetCore.Authorization;

namespace CustomersService.Presentation.Configuration
{
    public class CustomAuthorizeAttribute: AuthorizeAttribute
    {
        public CustomAuthorizeAttribute(Role[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
