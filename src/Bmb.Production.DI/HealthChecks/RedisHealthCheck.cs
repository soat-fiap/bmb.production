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
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var database = _connectionMultiplexer.GetDatabase();
            var result = await database.ExecuteAsync("PING");

            if (result.ToString() == "PONG")
            {
                return HealthCheckResult.Healthy("Redis is healthy");
            }

            return HealthCheckResult.Unhealthy("Redis is unhealthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis is unhealthy", ex);
        }
    }
}