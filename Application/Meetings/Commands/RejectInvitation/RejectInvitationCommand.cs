using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Meetings.Commands.RejectInvitation;

public class RejectInvitationCommand : IRequest<Unit>
{
    public Guid MeetingId { get; set; }
}

public class RejectInvitationCommandHandler : IRequestHandler<RejectInvitationCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public RejectInvitationCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    
    
    public async Task<Unit> Handle(RejectInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var meeting = await _dbContext
            .Meetings
            .Include(x => x.MeetingParticipants)
            .FirstOrDefaultAsync(x => x.Id == request.MeetingId, cancellationToken: cancellationToken);
        
        if (meeting is null) throw new AppException("Meeting is not found");
        
        var foundParticipant = meeting.MeetingParticipants.SingleOrDefault(x => x.ParticipantId == userId);
        if(foundParticipant is null || foundParticipant.InvitationStatus != InvitationStatus.Pending)
            throw new AppException("You don't have valid invitation");

        foundParticipant.InvitationStatus = InvitationStatus.Rejected;
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return await Task.FromResult(Unit.Value);
    }
}