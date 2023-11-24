using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Queries.GetRecentFriendMeetings;

public class GetRecentFriendMeetingsQuery : IRequest<List<MeetingPinDto>>
{
    public Guid FriendId { get; set; }
    public bool AsOrganizer { get; set; }
}

public class GetRecentFriendMeetingsQueryHandler : IRequestHandler<GetRecentFriendMeetingsQuery, List<MeetingPinDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUserContextService _userContextService;

    public GetRecentFriendMeetingsQueryHandler(IApplicationDbContext dbContext, IMapper mapper, IDateTimeProvider dateTimeProvider, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _userContextService = userContextService;
    }

    public async Task<List<MeetingPinDto>> Handle(GetRecentFriendMeetingsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var friend = await _dbContext
            .Users
            .Include(x => x.MeetingParticipants).ThenInclude(x => x.Meeting).ThenInclude(x => x.MeetingParticipants)
            .Include(x => x.MeetingParticipants).ThenInclude(x => x.Meeting).ThenInclude(x => x.Organizer)
            .Include(x => x.OrganizedEvents).ThenInclude(x => x.MeetingParticipants)
            .FirstOrDefaultAsync(x => x.Id == request.FriendId, cancellationToken);
        if (friend is null) throw new AppException("User you're looking for is not found");

        var friendRecentMeetings = friend.MeetingParticipants
            .Where(x => x.InvitationStatus == InvitationStatus.Accepted)
            .Where(x => x.Meeting!.EndDateTimeUtc < _dateTimeProvider.UtcNow)
            .Where(x => x.Meeting!.Visibility == MeetingVisibility.Public || x.Meeting.MeetingParticipants.Any(participant => participant.ParticipantId == userId && participant.InvitationStatus == InvitationStatus.Accepted))
            .OrderByDescending(x => x.Meeting!.StartDateTimeUtc)
            .Select(x => x.Meeting)
            .ToList();

        var friendRecentOrganizedMeeting = friend
            .OrganizedEvents
            .Where(x => x.EndDateTimeUtc < _dateTimeProvider.UtcNow)
            .Where(x => x.Visibility == MeetingVisibility.Public || x.MeetingParticipants.Any(participant =>
                participant.ParticipantId == userId && participant.InvitationStatus == InvitationStatus.Accepted))
            .ToList();


        List<MeetingPinDto> meetingsPins;

        if (request.AsOrganizer)
        {
            meetingsPins = _mapper.Map<List<MeetingPinDto>>(friendRecentOrganizedMeeting);
        }
        else
        {
            meetingsPins = _mapper.Map<List<MeetingPinDto>>(friendRecentMeetings);
        }
        
        return meetingsPins;
    }
}


