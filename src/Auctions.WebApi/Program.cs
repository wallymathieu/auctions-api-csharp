using System.Reflection;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Wallymathieu.Auctions.Infrastructure.Json;
using Wallymathieu.Auctions.Infrastructure.Models;
using Wallymathieu.Auctions.Infrastructure.Web;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;
using Wallymathieu.Auctions.WebApi;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(opts => { opts.JsonSerializerOptions.AddAuctionConverters(); });
builder.Services.AddApiVersioning(
                    options =>
                    {
                        // reporting api versions will return the headers
                        // "api-supported-versions" and "api-deprecated-versions"
                        options.ReportApiVersions = true;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.DefaultApiVersion = new ApiVersion(1, 0);
                        options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
                    }).AddApiExplorer(
                    options =>
                    {
                        // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                        // note: the specified format code will format the version as "'v'major[.minor][-status]"
                        options.GroupNameFormat = "'v'VVV";

                    });
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();
    var webAssembly = typeof(Program).GetTypeInfo().Assembly;
    //Set the comments path for the swagger json and ui.
    var xmlPath = webAssembly.Location
        .Replace(".dll", ".xml", StringComparison.OrdinalIgnoreCase)
        .Replace(".exe", ".xml", StringComparison.OrdinalIgnoreCase);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});
builder.AddAuctionsWebInfrastructure();
builder.Services.AddAuctionsWebJwt()
    .AddHttpContextAccessor()
    .AddHttpContextUserContext();
builder.Services.AddAuctionMapper();
builder.Services.AddOptions<PayloadAuthenticationOptions>();
//#if DEBUG // Only for development since it otherwise assumes that the network is 100% secure
builder.Services
    .AddAuthentication()
    .AddPayloadAuthentication(c =>
    {
        var principalHeader = builder.Configuration["PrincipalHeader"];
        if (!string.IsNullOrEmpty(principalHeader)) c.PrincipalHeader = principalHeader;
    });
//#else
// TODO: Register JWT based auth
//#endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

    app.UseSwaggerUI(
        options =>
        {
            var descriptions = app.DescribeApiVersions();

            // build a swagger endpoint for each discovered API version
            foreach (var description in descriptions)
            {
                Console.WriteLine($"Adding {description.GroupName}");
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
        });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();