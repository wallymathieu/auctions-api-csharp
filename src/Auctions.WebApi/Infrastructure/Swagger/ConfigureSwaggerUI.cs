using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Wallymathieu.Auctions.Api.Infrastructure.Swagger;

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