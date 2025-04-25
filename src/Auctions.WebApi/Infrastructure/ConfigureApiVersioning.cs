using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;

namespace Wallymathieu.Auctions.Api;

internal class ConfigureApiVersioning :
    IConfigureOptions<ApiVersioningOptions>,
    IConfigureOptions<ApiExplorerOptions>
{
    public void Configure(ApiVersioningOptions options)
    {
        // reporting api versions will return the headers
        // "api-supported-versions" and "api-deprecated-versions"
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
    }

    public void Configure(ApiExplorerOptions options)
    {
        // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
        // note: the specified format code will format the version as "'v'major[.minor][-status]"
        options.GroupNameFormat = "'v'VVV";
    }
}