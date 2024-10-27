using Bmb.Production.Application.Dtos;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model;

namespace Bmb.Production.Application;

public class GetKitchenLineUseCase(IKitchenOrderRepository kitchenOrderRepository) : IGetKitchenLineUseCase
{
    public async Task<KitchenQueueResponse> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var receivedOrdersTask = kitchenOrderRepository.GetAllAsync(KitchenQueue.Received, cancellationToken);
        var inPreparationOrdersTask = kitchenOrderRepository.GetAllAsync(KitchenQueue.InPreparation, cancellationToken);
        var readyOrdersTask = kitchenOrderRepository.GetAllAsync(KitchenQueue.Ready, cancellationToken);

        await Task.WhenAll(receivedOrdersTask, inPreparationOrdersTask, readyOrdersTask);

        return new KitchenQueueResponse(receivedOrdersTask.Result, inPreparationOrdersTask.Result,
            readyOrdersTask.Result);
    }
}