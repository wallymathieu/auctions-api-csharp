namespace Wallymathieu.Auctions;

/// <summary>
/// Inspired by Axon https://docs.axoniq.io/reference-guide/axon-framework/axon-framework-commands/modeling/aggregate
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class CommandHandlerAttribute : Attribute
{
}