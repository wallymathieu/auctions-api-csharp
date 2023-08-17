using Microsoft.Extensions.DependencyInjection;

namespace Wallymathieu.Auctions.Tests.Helpers;

public interface IDatabaseContextSetup
{
    void Use(IServiceCollection services);
    void Migrate(IServiceScope serviceScope);
    void TryRemove();
}