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

    public void Init(Type testClass, string testName)
    {
        var db = $"{testClass.Name}{testName}";
        _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Strong_password_123!")
            .WithHostname(db)
            .Build();
        _dbContainer.StartAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    public void Use(IServiceCollection services)
    {
        services.Remove(services.First(s => s.ServiceType == typeof(AuctionDbContext)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions<AuctionDbContext>)));
        services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions)));
        services.AddDbContext<AuctionDbContext>(c =>
            c.UseSqlServer(_dbContainer!.GetConnectionString(), opt => opt.MigrationsAssembly(Migrations.AssemblyName)));
    }

    public void Migrate(IServiceScope serviceScope)
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        context.Database.Migrate();
    }

    public void TryRemove()
    {
        _dbContainer?.DisposeAsync()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}