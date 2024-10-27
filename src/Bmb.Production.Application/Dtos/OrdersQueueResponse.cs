namespace Bmb.Production.Application.Dtos;

public record KitchenQueueResponse(IReadOnlyCollection<string> Received, IReadOnlyCollection<string> InPreparation, IReadOnlyCollection<string> Ready);