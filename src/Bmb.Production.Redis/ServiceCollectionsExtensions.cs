using System.Diagnostics.CodeAnalysis;
using System.Net;
using Bmb.Production.Core.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Bmb.Production.Redis;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IKitchenOrderRepository, KitchenOrderRepository>();

        var redisSettings = new RedisSettings();
        configuration.GetSection("RedisSettings").Bind(redisSettings);

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints =
                {
                    new DnsEndPoint(redisSettings.Host, redisSettings.Port)
                },
                AbortOnConnectFail = false,
            }));
    }
}