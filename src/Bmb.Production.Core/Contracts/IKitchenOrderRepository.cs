using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Core.Contracts;

public interface IKitchenOrderRepository
{
    Task SaveAsync(KitchenOrderDto order, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all orders grouped by status
    /// </summary>
    /// <param name="cancellationToken">Cancellation token. </param>
    /// <returns></returns>
    Task<(List<string> Received, List<string> InPreparation, List<string> Ready)> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get order by Id
    /// </summary>
    /// <param name="orderId">Order id</param>
    /// <param name="cancellationToken">Cancellation token. </param>
    /// <returns></returns>
    Task<KitchenOrderDto> GetAsync(Guid orderId, CancellationToken cancellationToken = default);
}