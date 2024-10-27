using System.Text.Json;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Bmb.Production.Redis.Repository;

public class RedisRepository : IKitchenOrderRepository
{
    private readonly ILogger<RedisRepository> _logger;
    private readonly IDatabase _database;

    public RedisRepository(ILogger<RedisRepository> logger, IConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _database = connectionMultiplexer.GetDatabase();
    }

    public Task SaveAsync(KitchenOrderDto order, CancellationToken cancellationToken = default)
    {
        return _database.StringSetAsync($"kitchen_order_{order.OrderId}", JsonSerializer.Serialize(order));
    }

    public Task<IReadOnlyCollection<string>> GetAllAsync(KitchenQueue queue,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<KitchenOrderDto?> GetAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var response = await _database.StringGetAsync($"kitchen_order_{orderId}");
        return response.HasValue ? JsonSerializer.Deserialize<KitchenOrderDto>(response!) : null;
    }
}