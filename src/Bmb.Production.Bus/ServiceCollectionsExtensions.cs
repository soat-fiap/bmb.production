using System.Diagnostics.CodeAnalysis;
using Bmb.Production.Core.Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bmb.Production.Bus;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddPaymentBus(this IServiceCollection services)
    {
        services.AddMassTransit(bus =>
        {
            bus.AddConsumer<OrderCreatedConsumer>();
            bus.UsingAmazonSqs((context, cfg) =>
            {
                cfg.Host("us-east-1", _ => { });
                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(false));
            });

            bus.ConfigureHealthCheckOptions(options =>
            {
                options.Name = "masstransit";
                options.MinimalFailureStatus = HealthStatus.Unhealthy;
                options.Tags.Add("health");
            });
        });

        services.AddScoped<IDispatcher, Dispatcher>();
    }
}