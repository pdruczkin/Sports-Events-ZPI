using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Commands.JoinMeeting;

public class JoinMeetingCommand : IRequest<Unit>
{
    public Guid MeetingId { get; set; }
}

public class JoinMeetingCommandHandler : IRequestHandler<JoinMeetingCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JoinMeetingCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _dateTimeProvider = dateTimeProvider;
    }
    
    
    public async Task<Unit> Handle(JoinMeetingCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var meeting = await _dbContext
            .Meetings
            .Include(x => x.MeetingParticipants)
            .FirstOrDefaultAsync(x => x.Id == request.MeetingId, cancellationToken: cancellationToken);
        
        if (meeting is null) throw new AppException("Meeting is not found");

        if (meeting.StartDateTimeUtc < _dateTimeProvider.UtcNow)
            throw new AppException("Joining to meeting is possible before meeting start time.");
        
        
        var userAge = _dateTimeProvider.CalculateAge(user.DateOfBirth);
        var isUserAgeCorrect = userAge >= meeting.MinParticipantsAge;

        if (!isUserAgeCorrect)
            throw new AppException("The user does not meet the meeting's min age requirements.");

        var currentParticipantsQuantity = 1 + await _dbContext // 1 - meeting's organizer is also a participant
            .MeetingParticipants
            .CountAsync(mp => mp.MeetingId == meeting.Id && mp.InvitationStatus == InvitationStatus.Accepted, cancellationToken: cancellationToken);

        if (currentParticipantsQuantity >= meeting.MaxParticipantsQuantity)
            throw new AppException("Max participants quantity reached, joining not available.");

        var friendshipWithOrganizer = await _dbContext.Friendships
            .Where(x => x.InviterId == meeting.OrganizerId && x.InviteeId == userId)
            .OrderByDescending(x => x.StatusDateTimeUtc)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if(friendshipWithOrganizer is not null && friendshipWithOrganizer.FriendshipStatus == FriendshipStatus.Blocked) 
            throw new AppException("You're not allowed to join that meeting."); 
        
        
        
        var foundParticipant = meeting.MeetingParticipants.SingleOrDefault(x => x.ParticipantId == userId);
        
        
        if (meeting.Visibility == MeetingVisibility.Public)
        {
            if (foundParticipant is null)
            {
                meeting.MeetingParticipants.Add(new MeetingParticipant
                {
                    Participant = user,
                    Meeting = meeting,
                    InvitationStatus = InvitationStatus.Accepted
                });
            }
            else
            {
                if (foundParticipant.InvitationStatus == InvitationStatus.Pending)
                {
                   foundParticipant.InvitationStatus = InvitationStatus.Accepted; 
                }
            }
        }
        else
        {
            if (foundParticipant is null || foundParticipant.InvitationStatus != InvitationStatus.Pending)
            {
                throw new AppException("You're not allowed to join that meeting");
            }

            foundParticipant.InvitationStatus = InvitationStatus.Accepted;
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }
}