using Bmb.Production.Application.UseCases;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Application;

public class ReplicateOrderUseCase(IKitchenOrderRepository kitchenOrderRepository) : IReplicateOrderUseCase
{
    public async Task ExecuteAsync(KitchenOrderDto order, CancellationToken cancellationToken = default)
    {
        var existingOrder = await kitchenOrderRepository.GetAsync(order.OrderId, cancellationToken);
        if (existingOrder is null)
            await kitchenOrderRepository.SaveAsync(order, cancellationToken);
    }
}