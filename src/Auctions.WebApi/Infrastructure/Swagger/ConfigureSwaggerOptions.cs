using System.Reflection;
using System.Text;
using System.Text.Json;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Json;

namespace Wallymathieu.Auctions.Api.Infrastructure.Swagger;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
internal class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) :
    IConfigureOptions<SwaggerGenOptions>,
    IConfigureOptions<SwaggerOptions>
{
    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        var webAssembly = typeof(Program).GetTypeInfo().Assembly;
        var informationalVersion =
            (webAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute))
                as AssemblyInformationalVersionAttribute[])?.First().InformationalVersion;

        var opts = new JsonSerializerOptions().AddAuctionConverters();
        options.MapType(typeof(Amount), () => new OpenApiSchema
        {
            Type = "string",
            Example = new OpenApiString(Amount.Zero(CurrencyCode.VAC).ToString())
        });
        options.MapType<TimeSpan>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "^(\\d{2}:)?\\d{2}:\\d{2}:\\d{2}$",
            Example = new OpenApiString(JsonSerializer.Serialize(TimeSpan.FromSeconds(1234), opts))
        });
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description, informationalVersion));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description, string? informationalVersion)
    {
        var text = new StringBuilder("Simple implementation of Auction API in C#");
        var info = new OpenApiInfo
        {
            Version = description.ApiVersion.ToString(),
            Title = "Auction API",
            Contact = new OpenApiContact
            {
                Name = "Oskar Gewalli",
                Email = "wallymathieu@users.noreply.github.com",
                Url = new Uri("https://github.com/wallymathieu/auctions-api-csharp")
            }
        };

        if (description.IsDeprecated)
        {
            text.Append(" This API version has been deprecated.");
        }

        if (description.SunsetPolicy is { } policy)
        {
            if (policy.Date is { } when)
            {
                text.Append(" The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
            }

            if (policy.HasLinks)
            {
                text.AppendLine();

                var rendered = false;

                for (var i = 0; i < policy.Links.Count; i++)
                {
                    var link = policy.Links[i];

                    if (link.Type == "text/html")
                    {
                        if (!rendered)
                        {
                            text.Append("<h4>Links</h4><ul>");
                            rendered = true;
                        }

                        text.Append("<li><a href=\"");
                        text.Append(link.LinkTarget.OriginalString);
                        text.Append("\">");
                        text.Append(
                            StringSegment.IsNullOrEmpty(link.Title)
                            ? link.LinkTarget.OriginalString
                            : link.Title.ToString());
                        text.Append("</a></li>");
                    }
                }

                if (rendered)
                {
                    text.Append("</ul>");
                }
            }
        }

        text.Append("<h4>Additional Information</h4>");
        text.Append("<p> Informational version: ");
        text.Append(informationalVersion ?? "dev");
        text.Append("</p>");
        info.Description = text.ToString();

        return info;
    }

    public void Configure(SwaggerOptions options)
    {
        options.RouteTemplate = "swagger/{documentName}/swagger.json";
    }
}
