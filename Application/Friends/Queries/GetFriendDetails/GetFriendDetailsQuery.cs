using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Friends.Queries.GetFriendDetails;

public class GetFriendDetailsQuery : IRequest<FriendDetailsDto>
{
    public Guid Id { get; set; }
}

public class GetFriendDetailsQueryHandler : IRequestHandler<GetFriendDetailsQuery, FriendDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetFriendDetailsQueryHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }
    
    public async Task<FriendDetailsDto> Handle(GetFriendDetailsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .Include(x => x.AsInvitee)
            .Include(x => x.AsInviter)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user is null) throw new AppException("User is not found");

        var friend = await _dbContext
            .Users
            .Include(x => x.Image)
            .Include(x => x.MeetingParticipants).ThenInclude(x => x.Meeting).ThenInclude(x => x.MeetingParticipants)
            .Include(x => x.MeetingParticipants).ThenInclude(x => x.Meeting).ThenInclude(x => x.Organizer)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (friend is null) throw new AppException("User you're looking for is not found");
        
        var friendDetailsDto = _mapper.Map<FriendDetailsDto>(friend);

        var recentMeetings = friend.MeetingParticipants
            .Where(x => x.InvitationStatus == InvitationStatus.Accepted)
            .Where(x => x.Meeting!.EndDateTimeUtc < _dateTimeProvider.UtcNow)
            .Where(x => x.Meeting!.Visibility == MeetingVisibility.Public || x.Meeting.MeetingParticipants.Any(participant => participant.ParticipantId == userId && participant.InvitationStatus == InvitationStatus.Accepted))
            .OrderByDescending(x => x.Meeting!.StartDateTimeUtc)
            .Take(1)
            .Select(x => x.Meeting)
            .ToList();

        friendDetailsDto.RecentMeetings = _mapper.Map<List<MeetingPinDto>>(recentMeetings);
        
        var lastFriendship = user.AsInvitee.Where(x => x.InviterId == request.Id)
            .Union(user.AsInviter.Where(x => x.InviteeId == request.Id)).MaxBy(x => x.StatusDateTimeUtc);

        var ifLimitData = false;

        if (lastFriendship == null) 
            ifLimitData = true;

        if (lastFriendship is not null && lastFriendship.FriendshipStatus != FriendshipStatus.Accepted)
            ifLimitData = true;
        
        if (ifLimitData)
        {
            friendDetailsDto.FirstName = null;
            friendDetailsDto.LastName = null;
            friendDetailsDto.Age = null;
            friendDetailsDto.Gender = null;
        }

        friendDetailsDto.FriendshipStatusDto.Status = lastFriendship is null ? null : lastFriendship.FriendshipStatus;
        friendDetailsDto.FriendshipStatusDto.IsOriginated = lastFriendship is null ? null : lastFriendship.InviterId == userId;

        return friendDetailsDto;
    }
}