using Auctions.Services;

namespace Tests.Helpers;

internal class FakeTime : ITime
{
    private readonly DateTimeOffset _now;

    public FakeTime(DateTimeOffset now)
    {
        _now = now;
    }

    public DateTimeOffset Now => _now;
}