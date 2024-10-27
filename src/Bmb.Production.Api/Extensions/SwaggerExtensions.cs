using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Bmb.Production.Api.Extensions;

[ExcludeFromCodeCoverage]
internal static class SwaggerExtensions
{
    internal static void SetupSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Kitchen Line API", Version = "v1", Extensions =
                {
                    {
                        "x-logo",
                        new OpenApiObject
                        {
                            {
                                "url",
                                new OpenApiString(
                                    "https://avatars.githubusercontent.com/u/165858718?s=384")
                            },
                            {
                                "background",
                                new OpenApiString(
                                    "#FF0000")
                            }
                        }
                    }
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
    }
}