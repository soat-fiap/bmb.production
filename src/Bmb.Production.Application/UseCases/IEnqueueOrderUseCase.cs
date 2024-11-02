namespace Bmb.Production.Application.UseCases;

public interface IEnqueueOrderUseCase
{
    Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default);
}