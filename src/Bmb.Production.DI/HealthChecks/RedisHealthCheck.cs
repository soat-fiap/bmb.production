using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Bmb.Production.DI.HealthChecks;

[ExcludeFromCodeCoverage]
public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer ??
                                 throw new ArgumentNullException(nameof(connectionMultiplexer));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_connectionMultiplexer.IsConnected)
            {
                return HealthCheckResult.Unhealthy(
                    "Redis connection is not established",
                    data: new Dictionary<string, object>
                    {
                        ["endpoint"] = _connectionMultiplexer.Configuration
                    });
            }

            var database = _connectionMultiplexer.GetDatabase();
            var result = await database.ExecuteAsync("PING")
                .WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);
            var response = result?.ToString();
            if (response == "PONG")
            {
                return HealthCheckResult.Healthy("Redis is healthy",
                    data: new Dictionary<string, object>
                    {
                        ["endpoint"] = _connectionMultiplexer.Configuration
                    });
            }

            return HealthCheckResult.Unhealthy($"Unexpected Redis response: {response}",
                data: new Dictionary<string, object>
                {
                    ["response"] = response,
                    ["endpoint"] = _connectionMultiplexer.Configuration
                });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Redis health check failed",
                ex,
                data: new Dictionary<string, object>
                {
                    ["endpoint"] = _connectionMultiplexer.Configuration
                });
        }
    }
}