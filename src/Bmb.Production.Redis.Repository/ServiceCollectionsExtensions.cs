using System.Diagnostics.CodeAnalysis;
using Bmb.Production.Core.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Bmb.Production.Redis.Repository;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IKitchenOrderRepository, RedisRepository>();

        var redisSettings = new RedisSettings();
        configuration.GetSection("RedisSettings").Bind(redisSettings);

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints =
                {
                    $"{redisSettings.Host}:{redisSettings.Port}"
                },
                AbortOnConnectFail = false,
            }));
    }
}