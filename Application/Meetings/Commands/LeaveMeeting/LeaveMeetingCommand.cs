using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Commands.LeaveMeeting;

public class LeaveMeetingCommand : IRequest<Unit>
{
    public Guid MeetingId { get; set; }
}

public class LeaveMeetingCommandHandler : IRequestHandler<LeaveMeetingCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LeaveMeetingCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _dateTimeProvider = dateTimeProvider;
    }
    
    
    public async Task<Unit> Handle(LeaveMeetingCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .Include(x => x.MeetingParticipants).ThenInclude(z => z.Meeting)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user is null) throw new AppException("User is not found");


        var meetingParticipation = user
            .MeetingParticipants
            .FirstOrDefault(x => x.MeetingId == request.MeetingId);

        if (meetingParticipation is null) throw new AppException("Meeting is not found");
        if (meetingParticipation.InvitationStatus != InvitationStatus.Accepted)
            throw new AppException("You can't leave that meeting");

        if (meetingParticipation.Meeting!.StartDateTimeUtc < _dateTimeProvider.UtcNow)
            throw new AppException("You can't leave meeting which has already started");

        meetingParticipation.InvitationStatus = InvitationStatus.Rejected;

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return await Task.FromResult(Unit.Value);
    }
}

