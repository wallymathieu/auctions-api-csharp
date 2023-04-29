using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wallymathieu.Auctions.Infrastructure.Cache;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Services;

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
