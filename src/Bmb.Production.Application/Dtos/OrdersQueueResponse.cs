namespace Bmb.Production.Application.Dtos;

public record KitchenQueueResponse(IReadOnlyCollection<KitchenQueueItem> Queued, IReadOnlyCollection<KitchenQueueItem> InPreparation, IReadOnlyCollection<KitchenQueueItem> Ready);

public record KitchenQueueItem(Guid OrderId, string Code);