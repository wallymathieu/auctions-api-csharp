using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Tests.Helpers;

internal class FakeSystemClock : ISystemClock
{
    public FakeSystemClock(DateTimeOffset now)
    {
        Now = now;
    }

    public DateTimeOffset Now { get; set; }
}