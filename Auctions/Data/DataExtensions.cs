using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Data;

public static class DataExtensions
{
    public static IServiceCollection AddAuctionDbContextSqlServer(this IServiceCollection services, string? connection)
    {
        return services.AddDbContext<AuctionDbContext>(e=>
            e.UseSqlServer(connection, 
                opt=>opt.MigrationsAssembly("Auctions")));
    }
}