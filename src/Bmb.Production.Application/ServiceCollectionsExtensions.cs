using System.Diagnostics.CodeAnalysis;
using Bmb.Production.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Production.Application;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IGetKitchenLineUseCase, GetKitchenLineUseCase>()
            .AddScoped<IReplicateOrderUseCase, ReplicateOrderUseCase>()
            .AddScoped<IReceiveOrderUseCase, ReceiveOrderUseCase>();
    }
}