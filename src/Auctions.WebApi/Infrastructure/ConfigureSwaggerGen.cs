using System.Reflection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Wallymathieu.Auctions.Api;

internal class ConfigureSwaggerGen :
    IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.OperationFilter<SwaggerDefaultValues>();
        var webAssembly = typeof(Program).GetTypeInfo().Assembly;
        //Set the comments path for the swagger json and ui.
        var xmlPath = webAssembly.Location
            .Replace(".dll", ".xml", StringComparison.OrdinalIgnoreCase)
            .Replace(".exe", ".xml", StringComparison.OrdinalIgnoreCase);
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);
    }
}