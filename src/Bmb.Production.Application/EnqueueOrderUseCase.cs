using Bmb.Production.Application.UseCases;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model;

namespace Bmb.Production.Application;

public class EnqueueOrderUseCase(
    IKitchenOrderRepository kitchenOrderRepository,
    IUpdateOrderStatusUseCase updateOrderStatusUseCase)
    : OrderUseCaseBase(kitchenOrderRepository), IEnqueueOrderUseCase
{
    public async Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(orderId, cancellationToken);
        if (order is null)
            return;

        await updateOrderStatusUseCase.ExecuteAsync(orderId, KitchenOrderStatus.Queued, cancellationToken);
        await Repository.EnqueueOrderAsync(order, cancellationToken);
    }
}