using Bmb.Production.Application.Dtos;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Controllers.Contracts;

public interface IKitchenOrderService
{
    Task<KitchenQueueResponse> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    
    Task<bool> UpdateOrderStatusAsync(Guid orderId, KitchenOrderStatus status, CancellationToken cancellationToken = default);
    
    Task<KitchenOrderDto?> GetNextOrderAsync(CancellationToken cancellationToken = default);
}