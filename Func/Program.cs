using Auctions.Data;
using Auctions.Infrastructure.Cache;
using Auctions.Infrastructure.Data;
using Auctions.Infrastructure.Services;
using Auctions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((builder,services) =>
    {
        services
            .AddAuctionRepositoryCached()
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString(ConnectionStrings.Redis);
                options.InstanceName = CacheKeys.Prefix;
            })
            .AddAuctionDbContextSqlServer(builder.Configuration.GetConnectionString(ConnectionStrings.DefaultConnection))
            .AddAuctionServicesCached();
    })
    .Build();

host.Run();
