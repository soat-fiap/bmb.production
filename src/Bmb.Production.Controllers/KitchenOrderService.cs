using Bmb.Production.Application.Dtos;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Controllers.Contracts;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Controllers;

public class KitchenOrderService(
    IGetKitchenLineUseCase getKitchenLineUseCase,
    IUpdateOrderStatusUseCase updateOrderStatusUseCase,
    IGetNextOrderUseCase getNextOrderUseCase) : IKitchenOrderService
{
    public Task<KitchenQueueResponse> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        return getKitchenLineUseCase.ExecuteAsync(cancellationToken);
    }

    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, KitchenOrderStatus status,
        CancellationToken cancellationToken = default)
    {
        await updateOrderStatusUseCase.ExecuteAsync(orderId, status, cancellationToken);
        return true;
    }

    public Task<KitchenOrderDto?> GetNextOrderAsync(CancellationToken cancellationToken = default)
    {
        return getNextOrderUseCase.ExecuteAsync(cancellationToken);
    }
}