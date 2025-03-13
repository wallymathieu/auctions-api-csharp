using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Wallymathieu.Auctions.Infrastructure.Data;

public static class DataExtensions
{
    internal static IServiceCollection AddAuctionQueryImplementation(
        this IServiceCollection services
    )
    {
        services.TryAddScoped<AuctionQuery>();
        return services;
    }

    public static IServiceCollection AddAuctionQueryNoCache(this IServiceCollection services)
    {
        return AddAuctionQueryImplementation(services)
            .AddScoped<IAuctionQuery>(c => c.GetRequiredService<AuctionQuery>());
    }

    public static IServiceCollection AddAuctionDbContextSqlServer(
        this IServiceCollection services,
        string? connection
    )
    {
        return services.AddDbContext<AuctionDbContext>(e =>
            e.UseSqlServer(connection, opt => opt.MigrationsAssembly(MigrationAssembly.Name))
        );
    }
}
