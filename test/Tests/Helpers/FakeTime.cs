using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Tests.Helpers;

internal sealed class FakeSystemClock(DateTimeOffset now) : ISystemClock
{
    public DateTimeOffset Now { get; set; } = now;
}