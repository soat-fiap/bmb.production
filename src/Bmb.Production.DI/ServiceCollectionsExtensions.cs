using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Production.DI;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void IoCSetup(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        // serviceCollection.ConfigurePersistenceApp(configuration);
        // serviceCollection.AddUseCases();
        // serviceCollection.AddControllers();
        // serviceCollection.AddOrdersGateway(configuration);
        // serviceCollection.AddDynamoDbConnection(configuration);
    }


    public static void ConfigureHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks();
    }
}   