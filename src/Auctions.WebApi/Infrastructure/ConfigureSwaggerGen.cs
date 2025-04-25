using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Wallymathieu.Auctions.Api;
internal class ConfigureSwaggerUI(IApiVersionDescriptionProvider provider) :
    IConfigureOptions<SwaggerUIOptions>
{
    public void Configure(SwaggerUIOptions options)
    {
        var descriptions = provider.ApiVersionDescriptions;

        // build a swagger endpoint for each discovered API version
        foreach (var (url,name) in from description in descriptions
                 select (
                     $"/swagger/{description.GroupName}/swagger.json",
                     description.GroupName.ToUpperInvariant()))
        {
            options.SwaggerEndpoint(url, name);
        }
    }
}
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