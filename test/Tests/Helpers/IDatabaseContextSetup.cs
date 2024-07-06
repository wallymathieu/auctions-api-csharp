using Microsoft.Extensions.DependencyInjection;

namespace Wallymathieu.Auctions.Tests.Helpers;

public interface IDatabaseFixture: IAsyncLifetime
{
    IDatabaseConfigurator Configurator { get; }
    IDatabaseMigrator Migrator { get; }
}

public interface IDatabaseConfigurator
{
    void Use(IServiceCollection services);
}

public interface IDatabaseMigrator
{
    void Migrate(IServiceScope serviceScope);
}