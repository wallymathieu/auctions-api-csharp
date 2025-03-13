namespace Wallymathieu.Auctions.Tests.Helpers;

public interface IDatabaseFixture : IAsyncLifetime
{
    IDatabaseConfigurator Configurator { get; }
    IDatabaseMigrator Migrator { get; }
}
