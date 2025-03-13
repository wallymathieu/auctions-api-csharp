using Microsoft.Extensions.DependencyInjection;

namespace Wallymathieu.Auctions.Tests.Helpers;

public interface IDatabaseConfigurator
{
    void Use(IServiceCollection services);
}
