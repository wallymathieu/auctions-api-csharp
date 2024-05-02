using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Cache;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Infrastructure.Services;
using Wallymathieu.Auctions.Infrastructure.Web.Queues;

namespace Wallymathieu.Auctions.Infrastructure.Web;

public static class WebApplicationBuilderExtensions
{
    public static void AddAuctionsWebInfrastructure(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        if (builder.Configuration.GetConnectionString(ConnectionStrings.Redis) != null)
        {
            builder.Services.AddAuctionQueryCached()
                .AddAuctionDbContextSqlServer(
                    builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection))
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
                .AddAuctionDbContextSqlServer(
                    builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection))
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
    }
}
