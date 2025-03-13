using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Tests.Helpers.Sqlite;

public class SqliteDatabaseConfigurator(string connectionString) : IDatabaseConfigurator
{
    public void Use(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        services.Remove(services.First(s => s.ServiceType == typeof(AuctionDbContext)));
        services.Remove(
            services.First(s => s.ServiceType == typeof(DbContextOptions<AuctionDbContext>))
        );
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions)));
        services.AddDbContext<AuctionDbContext>(c =>
        {
            c.UseSqlite(connectionString, opt => opt.MigrationsAssembly(MigrationAssembly.Name));
        });
    }
}
