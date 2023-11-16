using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IUserContextService _userContextService;
    private readonly IApplicationDbContext _dbContext;

    public ChatHub(IUserContextService userContextService, IApplicationDbContext dbContext)
    {
        _userContextService = userContextService;
        _dbContext = dbContext;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _userContextService.GetUserId;
        if (!userId.HasValue) throw new SignalRException($"Can't retrieve user Id");

        await Clients.All.SendAsync("ReceiveMessage", $"{userId.ToString()} has connected");
    }
    
    public async Task JoinChat(string meetingId)
    {
        var userId = _userContextService.GetUserId;
        if (!userId.HasValue) throw new SignalRException($"Can't retrieve user Id");

        var chatMessages = await _dbContext
            .ChatMessages
            .Where(x => x.MeetingId.ToString() == meetingId)
            .OrderByDescending(x => x.SentAtUtc)
            .ToListAsync();
        
        await Groups.AddToGroupAsync(Context.ConnectionId, meetingId);
        await Clients.Caller.SendAsync("History", chatMessages);
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