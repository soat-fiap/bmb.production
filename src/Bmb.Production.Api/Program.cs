using System.Text.Json.Serialization;
using Bmb.Production.Api.Auth;
using Bmb.Production.Api.Exceptions;
using Bmb.Production.Api.Extensions;
using Bmb.Production.DI;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
ILogger<Program>? logger = null;
try
{
    // Add services to the container.
    builder.Host.UseSerilog((context, configuration) =>
        configuration
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName));

    builder.Services.ConfigureJwt(builder.Configuration);
    builder.Services.AddAuthorization();
    builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins",
            builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });

    builder.Services.AddHttpLogging(_ => { });

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });;
    builder.Services.IoCSetup(builder.Configuration);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    builder.Services.SetupSwagger();


    var jwtOptions = builder.Configuration
        .GetSection("JwtOptions")
        .Get<JwtOptions>();
    builder.Services.AddSingleton(jwtOptions);
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.ConfigureHealthCheck();
    
    var app = builder.Build();
    logger = app.Services.GetService<ILogger<Program>>();
    app.UseHttpLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHealthChecks("/healthz", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    
    app.UseHttpsRedirection();

    app.UseCors("AllowSpecificOrigins");
    
    app.UseAuthentication();
    
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger?.LogCritical(ex, "Application start-up failed");
    Console.WriteLine(ex);
}
finally
{
    Log.CloseAndFlush();
}

