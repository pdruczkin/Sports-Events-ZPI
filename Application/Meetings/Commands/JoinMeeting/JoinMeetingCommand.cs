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

        if (meeting.Visibility == MeetingVisibility.Public)
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
            var foundParticipant = meeting.MeetingParticipants.SingleOrDefault(x => x.ParticipantId == userId);

            if (foundParticipant is null || meeting.StartDateTimeUtc < _dateTimeProvider.UtcNow || foundParticipant.InvitationStatus != InvitationStatus.Pending)
            {
                throw new AppException("You're not allowed to join that meeting");
            }

            foundParticipant.InvitationStatus = InvitationStatus.Accepted;
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }
}