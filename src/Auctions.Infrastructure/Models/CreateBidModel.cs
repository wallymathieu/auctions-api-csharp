using System.ComponentModel.DataAnnotations;

namespace Wallymathieu.Auctions.Infrastructure.Models;
/// <summary>
/// Note that the <see cref="CreateBidCommand"/> takes in an additional parameter that we want to include from the
/// route in our API instead of in the body.
/// </summary>
public record CreateBidModel([Required] Amount Amount);