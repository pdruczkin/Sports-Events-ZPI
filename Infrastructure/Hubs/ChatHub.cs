using Application.Common.Exceptions;
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
        if (!userId.HasValue) throw new SignalRException($"Can't retrieve user Id");
        
        await Clients.All.SendAsync("ReceiveMessage", $"{userId.Value} has connected");
    }

    public async Task JoinChat(string meetingId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, meetingId);
        
        var userId = _userContextService.GetUserId;
        if (!userId.HasValue) throw new SignalRException($"Can't retrieve user Id");
        
        await Clients.Group(meetingId.ToString())
            .SendAsync("ReceiveMessage", $"{userId.ToString()} has join meeting:{meetingId} chat");
    }
    
    public async Task LeaveChat(string meetingId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, meetingId);
        
        var userId = _userContextService.GetUserId;
        if (!userId.HasValue) throw new SignalRException($"Can't retrieve user Id");
        
        await Clients.Group(meetingId)
            .SendAsync("ReceiveMessage", $"{userId.ToString()} has left meeting:{meetingId} chat");
    }
}