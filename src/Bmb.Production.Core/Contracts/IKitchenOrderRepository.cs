using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Core.Contracts;

public interface IKitchenOrderRepository
{
    Task SaveAsync(KitchenOrderDto order, CancellationToken cancellationToken = default);
}