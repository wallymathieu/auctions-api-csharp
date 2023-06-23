using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Tests.Helpers;

internal class FakeTime : ITime
{
    public FakeTime(DateTimeOffset now)
    {
        Now = now;
    }

    public DateTimeOffset Now { get; set; }
}