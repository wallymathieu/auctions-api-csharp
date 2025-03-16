using Microsoft.Extensions.DependencyInjection;

namespace Wallymathieu.Auctions.Tests.Helpers;

public interface IDatabaseMigrator
{
    ValueTask Migrate(IServiceScope serviceScope);
}