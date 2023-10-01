using Microsoft.Extensions.DependencyInjection;

namespace Wallymathieu.Auctions.Tests.Helpers;

public interface IDatabaseContextSetup
{
    void Init(Type testClass, string testName);
    void Use(IServiceCollection services);
    void Migrate(IServiceScope serviceScope);
    void TryRemove();
}