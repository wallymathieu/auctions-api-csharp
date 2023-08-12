using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Tests.Helpers;

/// <summary>
/// Database context setup used to configure and setup the database context
/// </summary>
public class SqlLiteDatabaseContextSetup : IDatabaseContextSetup
{
    private readonly string _db;

    public SqlLiteDatabaseContextSetup(string db)
    {
        _db = db;
    }

    public void Use(IServiceCollection services)
    {
        services.Remove(services.First(s => s.ServiceType == typeof(AuctionDbContext)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions<AuctionDbContext>)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions)));
        services.AddDbContext<AuctionDbContext>(c =>
            c.UseSqlite("Data Source=" + _db, opt => opt.MigrationsAssembly(Migrations.AssemblyName)));
    }

    public void Migrate(IServiceScope serviceScope)
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        // NOTE, we would like to use :
        // context.Database.Migrate();
        // however we get the error
        // SQLite Error 1: 'AUTOINCREMENT is only allowed on an INTEGER PRIMARY KEY'.
        context.Database.EnsureCreated();
    }

    public void TryRemove()
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
    }
}