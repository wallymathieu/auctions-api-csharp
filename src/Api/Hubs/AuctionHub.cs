using Microsoft.AspNetCore.SignalR;

namespace Wallymathieu.Auctions.Api.Hubs;

public class AuctionHub: Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}