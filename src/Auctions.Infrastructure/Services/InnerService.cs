namespace Wallymathieu.Auctions.Infrastructure.Services;
/// <summary>
/// Intended to name internal registration in order to avoid getting the outer decorator when resolving the inner service.
/// See for instance <a href="https://autofac.readthedocs.io/en/latest/advanced/adapters-decorators.html#decorators">AutoFac decorators</a>.
/// </summary>
/// <param name="Service">instance of service</param>
/// <typeparam name="T">Service type</typeparam>
internal record InnerService<T>(T Service);