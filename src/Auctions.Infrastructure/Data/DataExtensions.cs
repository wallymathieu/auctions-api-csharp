using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wallymathieu.Auctions.Data;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Data.Cache;

namespace Wallymathieu.Auctions.Infrastructure.Data;

public static class DataExtensions
{
    private static IServiceCollection AddAuctionRepositoryImplementation(this IServiceCollection services)
    {
        services.TryAddScoped<AuctionRepository>();
        return services;
    }
    public static IServiceCollection AddAuctionRepositoryCached(this IServiceCollection services)
    {
        return AddAuctionRepositoryImplementation(services)
            .AddScoped<IAuctionRepository>(c=>new CachedAuctionRepository(
            c.GetRequiredService<IDistributedCache>(),
            c.GetRequiredService<AuctionDbContext>()))
            .AddScoped<IRepository<Auction>>(c=>c.GetRequiredService<IAuctionRepository>());
    }

    public static IServiceCollection AddAuctionRepositoryNoCache(this IServiceCollection services)
    {
        return AddAuctionRepositoryImplementation(services)
            .AddScoped<IAuctionRepository>(c=>
                c.GetRequiredService<AuctionRepository>())
            .AddScoped<IRepository<Auction>>(c=>c.GetRequiredService<IAuctionRepository>());
    }

    public static IServiceCollection AddAuctionDbContextSqlServer(this IServiceCollection services, string? connection)
    {
        return services.AddDbContext<AuctionDbContext>(e =>
            e.UseSqlServer(connection,
                opt => opt.MigrationsAssembly("Auctions.Infrastructure")));
    }
}