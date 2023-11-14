using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IUserContextService _userContextService;

    public ChatHub(IUserContextService userContextService)
    {
        _userContextService = userContextService;
    }
    
    public override async Task OnConnectedAsync()
    {
        var userId = _userContextService.GetUserId;
        await Clients.All.SendAsync("ReceiveMessage", $"{userId.ToString()} has joined");
    }
}