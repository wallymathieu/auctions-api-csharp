using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Cache;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Services;

namespace Wallymathieu.Auctions.Infrastructure.Web;

public static class WebApplicationBuilderExtensions
{
    public static void AddAuctionsWebInfrastructure(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (builder.Configuration.GetConnectionString("redis") != null)
        {
            builder.Services.AddAuctionQueryCached()
                .AddAuctionDbContextSqlServer(
                    builder.Configuration.GetConnectionString("auctions"))
                .AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = builder.Configuration.GetConnectionString("redis");
                    options.InstanceName = CacheKeys.Prefix;
                })
                .AddAuctionServicesCached();
        }
        else
        {
            builder.Services.AddAuctionQueryNoCache()
                .AddAuctionDbContextSqlServer(builder.Configuration.GetConnectionString("auctions"))
                .AddAuctionServicesNoCache();
        }

    }
}