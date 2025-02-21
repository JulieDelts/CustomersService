using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomersService.Application.Exceptions;
using CustomersService.Core;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CustomersService.Application.Services.ServicesUtils;

public class CustomerUtils(
    ICustomerRepository customerRepository,
    ILogger<CustomerUtils> logger,
    IOptions<AuthConfigOptions> options)
{
    public async Task<Customer> GetByIdAsync(Guid id)
    {
        logger.LogDebug("Retrieving customer with ID {CustomerId}", id);

        var customerDto = await customerRepository.GetByConditionAsync(c => c.Id == id);
        logger.LogTrace("Retrieved customer data: {@CustomerDto}", customerDto);

        if (customerDto == null)
        {
            logger.LogWarning("Customer with id {CustomerId} was not found", id);
            throw new EntityNotFoundException($"Customer with id {id} was not found.");
        }

        logger.LogDebug("Successfully retrieved customer with ID {CustomerId}", id);
        return customerDto;
    }

    public bool CheckPassword(string passworDtoCheck, string passwordHash)
    {
        logger.LogDebug("Checking password");
        return BCrypt.Net.BCrypt.EnhancedVerify(passworDtoCheck, passwordHash);
    }

    public string GenerateToken(Customer customer)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value?.Key ?? "SecurityKeyTestSecurityKeyTestSecurityKeyTests"));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            issuer: options.Value?.Issuer,
            audience: options.Value?.Audience,
            claims: new List<Claim>()
            {
                new Claim(ClaimTypes.Role, customer.Role.ToString()),
                new Claim("SystemId", customer.Id.ToString())
            },
            expires: DateTime.Now.AddMinutes(180),
            signingCredentials: signingCredentials
        );

        logger.LogInformation("Customer authenticated successfully with Id {CustomerId}", customer.Id);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}
