using Bmb.Domain.Core.Events.Integration;
using Bmb.Production.Application.UseCases;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bmb.Production.Bus.Consumers;

public class OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IReplicateOrderUseCase replicateOrderUseCase)
    : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        try
        {
            var message = context.GetValidIntegrationMessage();

            logger.LogInformation("Processing order {OrderId}", message.Id);

            await replicateOrderUseCase.ExecuteAsync(
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