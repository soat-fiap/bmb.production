using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Application.UseCases;

public interface IGetNextOrderUseCase
{
    Task<KitchenOrderDto?> ExecuteAsync(CancellationToken cancellationToken = default);
}