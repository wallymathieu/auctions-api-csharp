using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Wallymathieu.Auctions.Api;
using Wallymathieu.Auctions.Api.Infrastructure;
using Wallymathieu.Auctions.Api.Infrastructure.Swagger;
using Wallymathieu.Auctions.Api.Models;
using Wallymathieu.Auctions.Infrastructure.Json;
using Wallymathieu.Auctions.Infrastructure.Web;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddSqlServerClient(connectionName: "auctions");
builder.AddRedisClient("redis");

// Add services to the container.
builder.Services.AddHealthChecks();
builder.Services.AddControllers().AddJsonOptions(opts => { opts.JsonSerializerOptions.AddAuctionConverters(); });
builder.Services.AddApiVersioning().AddApiExplorer();
builder.Services
    .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
    .AddTransient<IConfigureOptions<SwaggerOptions>, ConfigureSwaggerOptions>()
    .AddTransient<IConfigureOptions<ApiVersioningOptions>, ConfigureApiVersioning>()
    .AddTransient<IConfigureOptions<ApiExplorerOptions>, ConfigureApiVersioning>()
    .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGen>()
    .AddTransient<IConfigureOptions<SwaggerUIOptions>, ConfigureSwaggerUI>();
builder.Services.AddSwaggerGen();
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
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();