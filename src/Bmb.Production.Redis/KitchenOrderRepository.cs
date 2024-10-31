using System.Text.Json;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Bmb.Production.Redis;

public class KitchenOrderRepository : IKitchenOrderRepository
{
    private readonly ILogger<KitchenOrderRepository> _logger;
    private readonly IDatabase _database;
    public static string KdsOrderKey = "kds_order";
    public static string KdsOrderStatusKey = "kds_order_status";
    public static string KdsReceivedOrdersKey = "kds_received";
    public static string KdsOrderSet = "kds_set";

    public KitchenOrderRepository(ILogger<KitchenOrderRepository> logger, IConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task SaveAsync(KitchenOrderDto order, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Saving order with ID {OrderId}", order.OrderId);
        await _database.HashSetAsync($"{KdsOrderKey}:{order.OrderId}", new HashEntry[]
        {
            new("value", JsonSerializer.Serialize(order))
        });
        _logger.LogInformation("Order with ID {OrderId} saved successfully", order.OrderId);
    }

    public async Task<IReadOnlyCollection<KitchenOrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all orders");
        var ordersSetKeys = (await _database.SetMembersAsync(KdsOrderSet))
            .Select(orderId => new RedisKey($"{KdsOrderKey}:{orderId}"));

        var orders = new List<KitchenOrderDto>();

        foreach (var key in ordersSetKeys)
        {
            var response = await _database.HashGetAsync(key, "value");
            if (response.HasValue)
            {
                orders.Add(JsonSerializer.Deserialize<KitchenOrderDto>(response!));
            }
        }

        _logger.LogInformation("Retrieved {OrderCount} orders", orders.Count);
        return orders;
    }

    public async Task<KitchenOrderDto?> GetAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving order with ID {OrderId}", orderId);
        var response = await _database.HashGetAsync($"{KdsOrderKey}:{orderId}", "value");
        if (!response.HasValue)
        {
            _logger.LogWarning("Order with ID {OrderId} not found", orderId);
            return null;
        }

        _logger.LogInformation("Order with ID {OrderId} retrieved successfully", orderId);
        return JsonSerializer.Deserialize<KitchenOrderDto>(response!);
    }

    public async Task EnqueueOrderAsync(KitchenOrderDto order, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Enqueuing order with ID {OrderId}", order.OrderId);

        var transaction = _database.CreateTransaction();
        await transaction.SetAddAsync(KdsOrderSet, order.OrderId.ToString());
        await transaction.ListLeftPushAsync(KdsReceivedOrdersKey, order.OrderId.ToString());
        await transaction.ExecuteAsync();
        
        _logger.LogInformation("Order with ID {OrderId} enqueued successfully", order.OrderId);
    }

    public async Task<KitchenOrderDto?> GetNextOrderAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving next order from queue");
        var response = await _database.ListRightPopAsync(KdsReceivedOrdersKey);

        if (!response.HasValue)
        {
            _logger.LogWarning("No orders in the queue");
            return default;
        }

        return JsonSerializer.Deserialize<KitchenOrderDto>(response!);
    }

    public async Task UpdateStatusAsync(Guid orderId, KitchenOrderStatus status,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating status of order with ID {OrderId} to {Status}", orderId, status);
        await _database.HashSetAsync($"{KdsOrderStatusKey}:{orderId}", new HashEntry[]
        {
            new("status", status.ToString())
        });
        _logger.LogInformation("Status of order with ID {OrderId} updated to {Status}", orderId, status);
    }

    public async Task DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting order with ID {OrderId}", orderId);
        await _database.KeyDeleteAsync($"{KdsOrderKey}:{orderId}");
        await _database.KeyDeleteAsync($"{KdsOrderStatusKey}:{orderId}");
        _logger.LogInformation("Order with ID {OrderId} deleted successfully", orderId);
    }
}