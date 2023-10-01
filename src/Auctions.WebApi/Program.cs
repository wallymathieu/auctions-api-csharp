using System.Reflection;
using System.Text.Json;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Wallymathieu.Auctions.Api.Models;
using Wallymathieu.Auctions.Api.Infrastructure.Queues;
using Wallymathieu.Auctions.Api.Middleware.Auth;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Cache;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Json;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Infrastructure.Services;
using Wallymathieu.Auctions.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.AddAuctionConverters();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var webAssembly = typeof(Program).GetTypeInfo().Assembly;
    var informationalVersion =
        (webAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute))
            as AssemblyInformationalVersionAttribute[])?.First().InformationalVersion;

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = informationalVersion ?? "dev",
        Title = "Auction API",
        Description = "Simple implementation of Auction API in C#",
        Contact = new OpenApiContact
        {
            Name = "Oskar Gewalli",
            Email = "wallymathieu@users.noreply.github.com",
            Url = new Uri("https://github.com/wallymathieu/auctions-api-csharp")
        }
    });

    //Set the comments path for the swagger json and ui.
    var xmlPath = webAssembly.Location.Replace(".dll", ".xml").Replace(".exe", ".xml");
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
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
});
if (builder.Configuration.GetConnectionString(ConnectionStrings.Redis) != null)
{
    builder.Services.AddAuctionRepositoryCached()
        .AddAuctionDbContextSqlServer(builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection))
        .AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString(ConnectionStrings.Redis);
            options.InstanceName = CacheKeys.Prefix;
        })
        .AddAuctionServicesCached();
}
else
{
    builder.Services.AddAuctionRepositoryNoCache()
        .AddAuctionDbContextSqlServer(builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection))
        .AddAuctionServicesNoCache();
}

var azureStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorage");
if (azureStorageConnectionString != null)
{
    // Register Azure Clients
    builder.Services.AddAzureClients(azureClientsBuilder =>
    {
        azureClientsBuilder.AddQueueServiceClient(azureStorageConnectionString).ConfigureOptions(queueOptions =>
        {
            queueOptions.MessageEncoding = Azure.Storage.Queues.QueueMessageEncoding.Base64;
        });

        azureClientsBuilder.UseCredential(new DefaultAzureCredential());
    });
}
builder.Services.AddScoped<IMessageQueue, AzureMessageQueue>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<AuctionMapper>();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddOptions<PayloadAuthenticationOptions>();
builder.Services.AddSingleton<ClaimsPrincipalParser>();
builder.Services.AddSingleton<JwtPayloadClaimsPrincipalParser>();
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

    app.UseReDoc(c =>
    {
        c.RoutePrefix = "docs";
        c.EnableUntrustedSpec();
        c.SpecUrl("/swagger/v1/swagger.json");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
