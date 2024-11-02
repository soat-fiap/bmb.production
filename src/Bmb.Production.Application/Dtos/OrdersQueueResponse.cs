namespace Bmb.Production.Application.Dtos;

public record KitchenQueueResponse(IReadOnlyCollection<string> Queued, IReadOnlyCollection<string> InPreparation, IReadOnlyCollection<string> Ready);