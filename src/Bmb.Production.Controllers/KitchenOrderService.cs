using Bmb.Production.Application.Dtos;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Controllers.Contracts;

namespace Bmb.Production.Controllers;

public class KitchenOrderService(IGetKitchenLineUseCase getKitchenLineUseCase) : IKitchenOrderService
{
    public Task<KitchenQueueResponse> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        return getKitchenLineUseCase.ExecuteAsync(cancellationToken);
    }
}