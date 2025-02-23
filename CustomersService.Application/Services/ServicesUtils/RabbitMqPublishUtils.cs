using CustomersService.Persistence.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;
using MYPBackendMicroserviceIntegrations.Messages;

namespace CustomersService.Application.Services.ServicesUtils;

public class RabbitMqPublishUtils(IPublishEndpoint publishEndpoint, ILogger<RabbitMqPublishUtils> logger)
{
    public async Task PublishAccountUpdateAsync(Account account)
    {
        logger.LogInformation("Sending account update with id {id} to RabbitMq", account.Id); 

        await publishEndpoint.Publish(new AccountMessage()
        {
            Id = account.Id,
            CustomerId = account.CustomerId,
            IsDeactivated = account.IsDeactivated,
            DateCreated = account.DateCreated,
            Currency = account.Currency
        });

        logger.LogInformation("Sent account update with id {id} to RabbitMq", account.Id);
    }

    public async Task PublishRoleUpdateIdsAsync(List<Guid> ids)
    {
        logger.LogInformation("Sending role update with ids {ids} to RabbitMq", string.Join(", ", ids));

        await publishEndpoint.Publish(ids);

        logger.LogInformation("Sent role update with ids {ids} to RabbitMq", string.Join(", ", ids));
    }

    public async Task PublishCustomerUpdateAsync(Customer customer)
    {
        logger.LogInformation("Sending customer update with id {id} to RabbitMq", customer.Id);

        await publishEndpoint.Publish(new CustomerMessage()
        {
            Id = customer.Id,
            Email = customer.Email,
            Role = customer.Role,
            Phone = customer.Phone,
            Address = customer.Address,
            Password = customer.Password,
            IsDeactivated = customer.IsDeactivated,
            BirthDate = customer.BirthDate,
            CustomVipDueDate = customer.CustomVipDueDate,
            FirstName = customer.FirstName,
            LastName = customer.LastName
        });

        logger.LogInformation("Sent customer update with id {id} to RabbitMq", customer.Id);
    }
}
