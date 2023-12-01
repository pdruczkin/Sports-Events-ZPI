using Application.Common;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Commands.DeleteMeeting;

public class DeleteMeetingCommand : IRequest<Unit>
{
    public Guid MeetingId { get; set; }
}

public class DeleteMeetingCommandHandler : IRequestHandler<DeleteMeetingCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEmailSender _emailSender;
    public DeleteMeetingCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IDateTimeProvider dateTimeProvider, IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _dateTimeProvider = dateTimeProvider;
        _emailSender = emailSender;
    }

    public async Task<Unit> Handle(DeleteMeetingCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var meeting = await _dbContext
            .Meetings
            .Include(x => x.Organizer)
            .Include(x => x.MeetingParticipants).ThenInclude(x => x.Participant)
            .FirstOrDefaultAsync(x => x.Id == request.MeetingId, cancellationToken);
        if (meeting is null) throw new AppException("Meeting is not found");

        if (meeting.StartDateTimeUtc < _dateTimeProvider.UtcNow)
            throw new AppException("Deleting a meeting is possible only before meeting start time.");

        if (user.Role != Role.Administrator && user.Id != meeting.OrganizerId)
            throw new AppException("Only organizer or system admin can delete meeting.");

        _dbContext.Meetings.Remove(meeting);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var participantsAccepted = meeting.MeetingParticipants.Where(x => x.InvitationStatus == InvitationStatus.Accepted).ToList();

        await SendEmails(meeting.Organizer, participantsAccepted, meeting.Title);

        return await Task.FromResult(Unit.Value);
    }

    private async Task SendEmails(User organizer, List<MeetingParticipant> meetingParticipants, string meetingTitle)
    {
        var emailDto = Mails.GetDeletedMeetingNotificationEmail(organizer.Email, organizer.Username, meetingTitle);
        await _emailSender.SendEmailAsync(emailDto);

        foreach (var meetingParticipant in meetingParticipants)
        {
            emailDto = Mails.GetDeletedMeetingNotificationEmail(meetingParticipant.Participant!.Email, meetingParticipant.Participant!.Username, meetingTitle);
            await _emailSender.SendEmailAsync(emailDto);
        }
    }

}


