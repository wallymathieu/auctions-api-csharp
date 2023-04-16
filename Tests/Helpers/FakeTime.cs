using Auctions.Services;

namespace Tests;

internal class FakeTime : ITime
{
    private readonly DateTimeOffset _now;

    public FakeTime(DateTimeOffset now)
    {
        _now = now;
    }

    public DateTimeOffset Now => this._now;
}