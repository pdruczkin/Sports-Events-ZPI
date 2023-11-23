using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IUserContextService _userContextService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public ChatHub(IUserContextService userContextService, IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider, IMapper mapper)
    {
        _userContextService = userContextService;
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _userContextService.GetUserId;
        if (!userId.HasValue) throw new SignalRException($"Can't retrieve user Id");

        await Clients.All.SendAsync("ConnectionCheck", $"{userId.ToString()} has connected");
    }
    
    public async Task JoinChat(string meetingId)
    {
        var userId = _userContextService.GetUserId;
        if (!userId.HasValue) throw new SignalRException($"Can't retrieve user Id");

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if(user == null) throw new SignalRException($"Can't find user of id: {userId}");
        
        var isValidGuid = Guid.TryParse(meetingId, out var meetingIdGuid);
        if(!isValidGuid) throw new SignalRException($"MeetingId must be of type Guid");
        
        var meeting = await _dbContext.Meetings.Include(x => x.MeetingParticipants).FirstOrDefaultAsync(x => x.Id == meetingIdGuid);
        if(meeting == null) throw new SignalRException($"Meeting of id {meetingId} is not found");
        
        if(!meeting.MeetingParticipants.Any(x => x.ParticipantId == userId && x.InvitationStatus == InvitationStatus.Accepted)) throw new SignalRException($"You're not allowed to join this chat");
        
        var chatMessages = await _dbContext
            .ChatMessages
            .Include(x => x.User)
            .Where(x => x.MeetingId.ToString() == meetingId)
            .OrderByDescending(x => x.SentAtUtc)
            .ToListAsync();

        var chatMessagesDto = _mapper.Map<IEnumerable<ChatMessageDto>>(chatMessages);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, meetingId);
        await Clients.Caller.SendAsync("History", chatMessagesDto);
        await Clients.All.SendAsync("ConnectionCheck", $"{userId.ToString()} has join chat with meetingId {meetingId}");
    }
    
    public async Task LeaveChat(string meetingId)
    {
        var userId = _userContextService.GetUserId;
        if (!userId.HasValue) throw new SignalRException($"Can't retrieve user Id");
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, meetingId);
        await Clients.All.SendAsync("ConnectionCheck", $"{userId.ToString()} has left chat with meetingId {meetingId}");
    }

    public async Task SendMessage(string meetingId, string message)
    {
        var userId = _userContextService.GetUserId;
        if (!userId.HasValue) throw new SignalRException($"Can't retrieve user Id");

        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if(user == null) throw new SignalRException($"Can't find user of id: {userId}");
        
        var isValidGuid = Guid.TryParse(meetingId, out var meetingIdGuid);
        if(!isValidGuid) throw new SignalRException($"MeetingId must be of type Guid");
        var meeting = await _dbContext.Meetings.FirstOrDefaultAsync(x => x.Id == meetingIdGuid);
        if(meeting == null) throw new SignalRException($"Meeting of id {meetingId} is not found");

        var chatMessage = new ChatMessage
        {
            User = user,
            Meeting = meeting,
            SentAtUtc = _dateTimeProvider.UtcNow,
            Value = message
        };

        _dbContext.ChatMessages.Add(chatMessage);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var chatMessageDto = _mapper.Map<ChatMessageDto>(chatMessage);
        
        await Clients.Group(meetingId).SendAsync("ReceiveMessage", chatMessageDto);
    }
}