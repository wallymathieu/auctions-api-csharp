using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Tests.Helpers;

/// <summary>
/// Database context setup used to configure and setup the database context
/// </summary>
public class MsSqlDatabaseContextSetup : IDatabaseContextSetup
{
    private MsSqlContainer? _dbContainer ;

    public Task Init(Type testClass, string testName)
    {
        ArgumentNullException.ThrowIfNull(testName, nameof(testName));
        ArgumentNullException.ThrowIfNull(testClass, nameof(testClass));

        var tinyhash = string.Format(CultureInfo.InvariantCulture, "{0:X}", testName.GetHashCode(StringComparison.Ordinal));
        var db = $"{testClass.Name}{tinyhash}";
        _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Strong_password_123!")
            .WithHostname(db)
            .Build();
        return _dbContainer.StartAsync();
    }

    public void Use(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Remove(services.First(s => s.ServiceType == typeof(AuctionDbContext)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions<AuctionDbContext>)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions)));
        services.AddDbContext<AuctionDbContext>(c =>
            c.UseSqlServer(_dbContainer!.GetConnectionString(), opt => opt.MigrationsAssembly(MigrationAssembly.Name)));
    }

    public void Migrate(IServiceScope serviceScope)
    {
        ArgumentNullException.ThrowIfNull(serviceScope);
        var context = serviceScope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        context.Database.Migrate();
    }

    public async Task TryRemove()
    {
        if (_dbContainer!=null)
            await _dbContainer.DisposeAsync();
    }
}