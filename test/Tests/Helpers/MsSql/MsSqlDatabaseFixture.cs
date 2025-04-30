using Testcontainers.MsSql;
using Xunit.Internal;

namespace Wallymathieu.Auctions.Tests.Helpers.MsSql;

/// <summary>
/// Database context setup used to configure and setup the database context
/// </summary>
public sealed class MsSqlDatabaseFixture : IDatabaseFixture
{
    private MsSqlContainer? _dbContainer;

    public async ValueTask InitializeAsync()
    {
        var db = $"{Guid.NewGuid():N}";
        _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Strong_password_123!")
            .WithHostname(db)
            .Build();
        await _dbContainer.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContainer != null)
            await _dbContainer.DisposeAsync();
    }

    public IDatabaseConfigurator Configurator
    {
        get
        {
            if (_dbContainer is null)
                throw new InvalidOperationException(
                    "Database not initialized");
            return new MsSqlDatabaseConfigurator(_dbContainer.GetConnectionString());
        }
    }

    public IDatabaseMigrator Migrator => new MsSqlDatabaseMigrator(_dbContainer.GetConnectionString());
}