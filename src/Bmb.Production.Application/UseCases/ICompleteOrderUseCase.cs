namespace Bmb.Production.Application.UseCases;

public interface ICompleteOrderUseCase
{
    Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default);
}