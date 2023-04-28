using System.Text.Json.Serialization;
using App.Middleware.Auth;
using Auctions.Data;
using Auctions.Domain;
using Auctions.Json;
using Auctions.Services;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers(c => c.Filters.Add<DecodedHeaderAuthorizationFilter>()).AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opts.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
    opts.JsonSerializerOptions.Converters.Add(new AmountConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.MapType(typeof(Amount), () => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString(Amount.Zero(CurrencyCode.VAC).ToString())
    });
});
if (builder.Configuration.GetConnectionString(ConnectionStrings.Redis) != null)
{
    builder.Services.AddAuctionRepositoryCached()
        .AddAuctionDbContextSqlServer(builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection))
        .AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString(ConnectionStrings.Redis);
            options.InstanceName = "auctions";
        })
        .AddAuctionServicesCached();
}
else
{
    builder.Services.AddAuctionRepositoryNoCache()
        .AddAuctionDbContextSqlServer(builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection))
        .AddAuctionServicesNoCache();
 
}

// Register Azure Clients
builder.Services.AddAzureClients(azureClientsBuilder => {
    var azureStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorage");    

    azureClientsBuilder.AddQueueServiceClient(azureStorageConnectionString).ConfigureOptions(queueOptions => {
        queueOptions.MessageEncoding = Azure.Storage.Queues.QueueMessageEncoding.Base64;
    });
    
    azureClientsBuilder.UseCredential(new DefaultAzureCredential());
});
#if DEBUG // Only for development since it otherwise assumes that the network is 100% secure 
builder.Services.AddSingleton<DecodedHeaderAuthorizationFilter>();
if (!string.IsNullOrEmpty(builder.Configuration["PrincipalHeader"]))
{
    builder.Services.AddSingleton<IClaimsPrincipalParser, ClaimsPrincipalParser>();
}
else
{
    builder.Services.AddSingleton<IClaimsPrincipalParser, JwtPayloadClaimsPrincipalParser>();
}
#else
// TODO: Register JWT based auth
#endif

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

app.Run();
