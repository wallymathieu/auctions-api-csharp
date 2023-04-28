using System.Text.Json.Serialization;
using App.Data;
using App.Middleware.Auth;
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
builder.Services.AddAuctionServices()
    .AddAuctionDbContextSqlServer(builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection))
    .AddAuctionRedisCache(builder.Configuration.GetConnectionString(ConnectionStrings.Redis));
// Register Azure Clients
builder.Services.AddAzureClients(azureClientsBuilder => {
    string azureStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorage");    

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
