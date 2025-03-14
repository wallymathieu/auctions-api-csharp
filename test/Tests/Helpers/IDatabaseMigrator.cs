using Microsoft.Extensions.DependencyInjection;

namespace Wallymathieu.Auctions.Tests.Helpers;

public interface IDatabaseMigrator
{
    Task Migrate(IServiceScope serviceScope);
}