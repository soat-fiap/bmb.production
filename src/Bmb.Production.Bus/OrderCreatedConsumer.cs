using Bmb.Domain.Core.Events.Integration;
using Bmb.Production.Core.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bmb.Production.Bus;

public class OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IKitchenOrderRepository kitchenOrderRepository)
    : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        try
        {
            var message = context.Message;

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "Order created message cannot be null");
            }

            logger.LogInformation("Processing order {OrderId}", message.Id);

            await kitchenOrderRepository.SaveAsync(
                message.ToDto(),
                context.CancellationToken);

            logger.LogInformation(
                "Successfully processed order {OrderId}",
                message.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process order created message: {ErrorMessage}", ex.Message);
            throw;
        }
    }
}