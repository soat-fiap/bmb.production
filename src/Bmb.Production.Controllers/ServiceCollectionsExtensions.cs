using System.Diagnostics.CodeAnalysis;
using Bmb.Production.Controllers.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Production.Controllers;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddControllers(this IServiceCollection services)
    {
        services.AddScoped<IKitchenOrderService, KitchenOrderService>();
    }
}