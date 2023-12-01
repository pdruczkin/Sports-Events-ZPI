using Application.Common;
using Application.Common.Exceptions;
using Application.Common.ExtensionMethods;
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
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEmailSender _emailSender;

    public SendInvitationCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IDateTimeProvider dateTimeProvider, IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _dateTimeProvider = dateTimeProvider;
        _emailSender = emailSender;
    }

    public async Task<Unit> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var newParticipant = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.NewParticipantId, cancellationToken);
        if (newParticipant is null) throw new AppException("Participant is not found");
        
        var friendshipWithOrganizer = await _dbContext.Friendships
            .Where(x => x.InviterId == userId && x.InviteeId == newParticipant.Id)
            .OrderByDescending(x => x.StatusDateTimeUtc)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if(friendshipWithOrganizer is not null && friendshipWithOrganizer.FriendshipStatus == FriendshipStatus.Blocked) 
            throw new AppException("Selected user is blocked, inviting is prohibited."); 
        
        var meeting = await _dbContext
            .Meetings
            .Include(x => x.MeetingParticipants)
            .FirstOrDefaultAsync(x => x.Id == request.MeetingId, cancellationToken: cancellationToken);
        if (meeting is null) throw new AppException("Meeting is not found");

        if (meeting.OrganizerId != userId) throw new ForbidException("Only organizer can invite new participants");

        var newParticipantAge = newParticipant.DateOfBirth.CalculateAge();
        var isNewParticipantAgeCorrect = newParticipantAge >= meeting.MinParticipantsAge;
        
        if(meeting.MeetingParticipants.Any(x => x.ParticipantId == newParticipant.Id && x.InvitationStatus is InvitationStatus.Accepted or InvitationStatus.Pending))
            throw new AppException("The invitation can't be send again");
        
        if(userId == newParticipant.Id) throw new AppException("You can't invite yourself");

        if(!isNewParticipantAgeCorrect) throw new AppException("The User doesn't meet the required age restriction");
        
        var currentParticipantsQuantity = 1 + await _dbContext // 1 - meeting's organizer is also a participant
            .MeetingParticipants
            .CountAsync(mp => mp.MeetingId == meeting.Id && mp.InvitationStatus == InvitationStatus.Accepted, cancellationToken: cancellationToken);

        if (currentParticipantsQuantity >= meeting.MaxParticipantsQuantity)
            throw new AppException("Max participants quantity reached, new invitations are not allowed.");

        var foundParticipation = meeting.MeetingParticipants.FirstOrDefault(x => x.ParticipantId == newParticipant.Id);
        if (foundParticipation is not null)
        {
            _dbContext.MeetingParticipants.Remove(foundParticipation);
        }
        
        var newMeetingParticipant = new MeetingParticipant
        {
            MeetingId = meeting.Id,
            ParticipantId = newParticipant.Id,
            InvitationStatus = InvitationStatus.Pending
        };
        
        meeting.MeetingParticipants.Add(newMeetingParticipant);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var emailDto = Mails.GetMeetingInvitationNotificationEmail(newParticipant.Email, newParticipant.Username, user.Username, meeting.Title, meeting.Id);
        await _emailSender.SendEmailAsync(emailDto);

        return await Task.FromResult(Unit.Value);
    }
}