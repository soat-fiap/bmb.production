﻿using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.Events;
using Bmb.Production.Bus.Consumers;
using Bmb.Production.Core.Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bmb.Production.Bus;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddBus(this IServiceCollection services)
    {
        services.AddMassTransit(bus =>
        {
            bus.AddConsumer<OrderCreatedConsumer>();
            bus.AddConsumer<OrderPaymentConfirmedConsumer>();
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