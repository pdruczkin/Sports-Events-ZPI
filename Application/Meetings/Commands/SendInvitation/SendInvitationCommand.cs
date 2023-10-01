using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Commands.SendInvitation;

public class SendInvitationCommand : IRequest<Unit>
{
    public Guid MeetingId { get; set; }
    public Guid NewParticipantId { get; set; }
}

public class SendInvitationCommandHandler : IRequestHandler<SendInvitationCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public SendInvitationCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    
    public async Task<Unit> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var newParticipant = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.NewParticipantId, cancellationToken);
        if (newParticipant is null) throw new AppException("Participant is not found");
        
        var meeting = await _dbContext
            .Meetings
            .Include(x => x.MeetingParticipants)
            .FirstOrDefaultAsync(x => x.Id == request.MeetingId, cancellationToken: cancellationToken);
        if (meeting is null) throw new AppException("Meeting is not found");


        if (meeting.OrganizerId != userId) throw new ForbidException("Only organizer can invite new participants");
        
        if (meeting.MeetingParticipants.Any(x => x.ParticipantId == newParticipant.Id) || userId == newParticipant.Id)
            throw new AppException("The invitation can't be send");

        var newMeetingParticipant = new MeetingParticipant
        {
            MeetingId = meeting.Id,
            ParticipantId = newParticipant.Id,
            InvitationStatus = InvitationStatus.Pending
        };
        
        meeting.MeetingParticipants.Add(newMeetingParticipant);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }
}