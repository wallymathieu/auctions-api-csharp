using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Tests.Helpers;

/// <summary>
/// Database context setup used to configure and setup the database context
/// </summary>
public class SqlLiteDatabaseContextSetup : IDatabaseContextSetup
{
    private string? _db;

    public Task Init(Type testClass, string testName)
    {
        ArgumentNullException.ThrowIfNull(testClass, nameof(testClass));
        ArgumentNullException.ThrowIfNull(testName, nameof(testName));

        _db = $"{testClass.Name}_{testName}.db";
        return Task.CompletedTask;
    }

    public void Use(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        services.Remove(services.First(s => s.ServiceType == typeof(AuctionDbContext)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions<AuctionDbContext>)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions)));
        services.AddDbContext<AuctionDbContext>(c =>
            c.UseSqlite("Data Source=" + _db, opt => opt.MigrationsAssembly(MigrationAssembly.Name)));
    }

    public void Migrate(IServiceScope serviceScope)
    {
        ArgumentNullException.ThrowIfNull(serviceScope, nameof(serviceScope));

        var context = serviceScope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        // NOTE, we would like to use :
        // context.Database.Migrate();
        // however we get the error
        // SQLite Error 1: 'AUTOINCREMENT is only allowed on an INTEGER PRIMARY KEY'.
        context.Database.EnsureCreated();
    }

    public Task TryRemove()
    {
        if (File.Exists(_db))
        {
            try
            {
                File.Delete(_db);
            }
            catch
            {
                // ignored
            }
        }
        return Task.CompletedTask;
    }
}