using Bmb.Production.Application.Dtos;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Application;

public class GetKitchenLineUseCase(IKitchenOrderRepository kitchenOrderRepository) : IGetKitchenLineUseCase
{
    public async Task<KitchenQueueResponse> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var orders = await kitchenOrderRepository.GetAllAsync(cancellationToken);

        var received = orders.Where(o => o.Status is KitchenOrderStatus.Queued);
        var inPreparation = orders.Where(o => o.Status is KitchenOrderStatus.Preparing);

        var ready = orders.Where(o => o.Status is KitchenOrderStatus.Ready);

        return new KitchenQueueResponse(
            GetOrderTrackingCode(received),
            GetOrderTrackingCode(inPreparation),
            GetOrderTrackingCode(ready));
    }

    private IReadOnlyCollection<string> GetOrderTrackingCode(IEnumerable<KitchenOrderDto> orders) =>
        orders.Select(o => o.OrderTrackingCode).ToList();
}