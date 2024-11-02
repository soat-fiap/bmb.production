using Bmb.Production.Core.Model;

namespace Bmb.Production.Application.UseCases;

public interface IUpdateOrderStatusUseCase
{
    Task ExecuteAsync(Guid orderId, KitchenOrderStatus status, CancellationToken cancellationToken = default);    
}