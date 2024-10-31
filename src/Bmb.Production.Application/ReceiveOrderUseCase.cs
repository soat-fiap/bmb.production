using Bmb.Production.Application.UseCases;
using Bmb.Production.Core.Contracts;

namespace Bmb.Production.Application;

public class ReceiveOrderUseCase(IKitchenOrderRepository kitchenOrderRepository) : IReceiveOrderUseCase
{
    public async Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await kitchenOrderRepository.GetAsync(orderId, cancellationToken);

        if (order is null)
            return;

        await kitchenOrderRepository.EnqueueOrderAsync(order!, cancellationToken);
    }
}