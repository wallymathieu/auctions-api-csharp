using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Wallymathieu.Auctions.Infrastructure.Data;

public static class DataExtensions
{
    internal static IServiceCollection AddAuctionRepositoryImplementation(this IServiceCollection services)
    {
        services.TryAddScoped<AuctionRepository>();
        return services;
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
                opt => opt.MigrationsAssembly(Migrations.AssemblyName)));
    }

}