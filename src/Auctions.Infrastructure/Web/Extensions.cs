using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Cache;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Infrastructure.Services;
using Wallymathieu.Auctions.Infrastructure.Web.Middleware.Auth;
using Wallymathieu.Auctions.Infrastructure.Web.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Infrastructure.Web;

public static class Extensions
{

    public static void AddAuctionsWebInfrastructure(this WebApplicationBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString(ConnectionStrings.Redis)))
        {
            builder.Services.AddAuctionQueryCached()
                .AddAuctionMartenStore(builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection)!)
                .AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = builder.Configuration.GetConnectionString(ConnectionStrings.Redis);
                    options.InstanceName = CacheKeys.Prefix;
                })
                .AddAuctionServicesCached();
        }
        else
        {
            builder.Services.AddAuctionQueryNoCache()
                .AddAuctionMartenStore(builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection)!)
                .AddAuctionServicesNoCache();
        }

        var azureStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorage");
        if (!string.IsNullOrWhiteSpace(azureStorageConnectionString))
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
    }

    public static IServiceCollection AddAuctionsWebJwt(this IServiceCollection services) =>
        services
            .AddSingleton<ClaimsPrincipalParser>()
            .AddSingleton<JwtPayloadClaimsPrincipalParser>();

    public static IServiceCollection AddHttpContextUserContext(this IServiceCollection services) =>
        services.AddScoped<IUserContext, UserContext>();
}