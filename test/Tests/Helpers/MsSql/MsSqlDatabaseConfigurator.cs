using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Tests.Helpers.MsSql;

public class MsSqlDatabaseConfigurator(string connectionString) : IDatabaseConfigurator
{
    public void Use(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Remove(services.First(s => s.ServiceType == typeof(AuctionDbContext)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions<AuctionDbContext>)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions)));
        services.AddDbContext<AuctionDbContext>(c =>
            c.UseSqlServer(connectionString, opt => opt.MigrationsAssembly(MigrationAssembly.Name)));
    }
}