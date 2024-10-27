using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Application.UseCases;

public interface IReplicateOrderUseCase
{
    Task ExecuteAsync(KitchenOrderDto order, CancellationToken cancellationToken = default);
}