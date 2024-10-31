using System.Diagnostics.CodeAnalysis;
using Bmb.Production.Application;
using Bmb.Production.Bus;
using Bmb.Production.Controllers;
using Bmb.Production.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Production.DI;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void IoCSetup(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddBus();
        serviceCollection.AddControllers();
        serviceCollection.AddDatabase(configuration);
        serviceCollection.AddUseCases();
    }

    public static void ConfigureHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks();
    }
}   