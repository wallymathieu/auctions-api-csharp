using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.ApiModels;
/// <summary>
/// Note <see cref="CreateBidCommand"/>
/// </summary>
public record CreateBidModel([Required] Amount Amount);