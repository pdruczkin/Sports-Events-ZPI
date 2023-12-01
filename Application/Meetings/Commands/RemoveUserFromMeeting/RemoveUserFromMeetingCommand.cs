using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Commands.RemoveUserFromMeeting;

public class RemoveUserFromMeetingCommand : IRequest<Unit>
{
    public Guid MeetingId { get; set; }
    public Guid UserToRemoveId { get; set; }
}

public class RemoveUserFromMeetingCommandHandler : IRequestHandler<RemoveUserFromMeetingCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEmailSender _emailSender;

    public RemoveUserFromMeetingCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IDateTimeProvider dateTimeProvider, IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _dateTimeProvider = dateTimeProvider;
        _emailSender = emailSender;
    }

    public async Task<Unit> Handle(RemoveUserFromMeetingCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var meeting = await _dbContext
            .Meetings
            .Include(x => x.MeetingParticipants).ThenInclude(x => x.Participant)
            .FirstOrDefaultAsync(x => x.Id == request.MeetingId, cancellationToken);
        if (meeting == null) throw new AppException("Meeting is not found");

        if (user.Role != Role.Administrator && user.Id != meeting.OrganizerId)
            throw new AppException("You are not allowed to remove anyone from this meeting");
        
        var userParticipationToRemove = meeting.MeetingParticipants.FirstOrDefault(x => x.ParticipantId == request.UserToRemoveId);
        if (userParticipationToRemove is null || (userParticipationToRemove.InvitationStatus != InvitationStatus.Accepted && userParticipationToRemove.InvitationStatus != InvitationStatus.Pending)) 
            throw new AppException("User to remove is not found in this meeting");
        
        if (meeting.StartDateTimeUtc < _dateTimeProvider.UtcNow)
            throw new AppException("You can't remove anyone from meeting when it has already started");

        userParticipationToRemove.InvitationStatus = InvitationStatus.Rejected;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var emailDto = Mails.GetRemovedFromMeetingNotificationEmail(userParticipationToRemove.Participant!.Email, userParticipationToRemove.Participant!.Username, user.Username, meeting.Title);
        await _emailSender.SendEmailAsync(emailDto);

        return await Task.FromResult(Unit.Value);
    }
}