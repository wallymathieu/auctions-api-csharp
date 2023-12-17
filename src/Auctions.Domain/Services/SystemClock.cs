namespace Wallymathieu.Auctions.Services;
/// <summary>
/// This is the concrete implementation of <see cref="ISystemClock"/>.
/// </summary>
public class SystemClock : ISystemClock
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}
/// <summary>
/// Note that we want to be able to fake time in tests.
/// </summary>
/// <remarks>
/// An alternative would be to use <a href="https://learn.microsoft.com/en-us/dotnet/api/system.timeprovider?view=net-8.0">System.TimeProvider</a>,
/// however the TimeProvider abstract class over-complicates the code significantly compared to a simple interface. You can see the dissent in the <a href="https://github.com/dotnet/runtime/issues/36617">proposal thread</a>. Most of the examples that the proposal references are simple interfaces.<br/>
/// Another alternative is to use <a href="https://nodatime.org/2.2.x/api/NodaTime.IClock.html">NodaTime.IClock</a>.
/// </remarks>
public interface ISystemClock
{
    DateTimeOffset Now { get; }
}