using Bmb.Production.Application.Dtos;

namespace Bmb.Production.Controllers.Contracts;

public interface IKitchenOrderService
{
    Task<KitchenQueueResponse> GetAllOrdersAsync(CancellationToken cancellationToken = default);
}