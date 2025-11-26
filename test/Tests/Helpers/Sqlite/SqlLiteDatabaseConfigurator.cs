using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Tests.Helpers.Sqlite;

public class SqliteDatabaseConfigurator(string connectionString) : IDatabaseConfigurator
{
    public void Use(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Remove(services.First(s => s.ServiceType == typeof(AuctionDbContext)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions<AuctionDbContext>)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions)));
        services.Remove(services.First(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<AuctionDbContext>)));
        services.AddDbContext<AuctionDbContext>(c =>
        {
            c.UseSqlite(connectionString, opt => opt.MigrationsAssembly(MigrationAssembly.Name));
        });
    }
}