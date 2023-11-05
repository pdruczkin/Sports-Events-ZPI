using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs;

public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
    }
}