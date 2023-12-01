using Application.Common;
using Application.Common.Exceptions;
using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Commands.UpdateMeetingData;

public class UpdateMeetingDataCommand : IRequest<Unit>, IMappable<Meeting>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; } // range: from -90 (South) to 90 (North)
    public double Longitude { get; set; } // range: from -180 (West) to 180 (East)
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime EndDateTimeUtc { get; set; }
    public MeetingVisibility Visibility { get; set; }
    public SportsDiscipline SportsDiscipline { get; set; }
    public Difficulty Difficulty { get; set; }
    public int MaxParticipantsQuantity { get; set; }
    public int MinParticipantsAge { get; set; }
}

public class UpdateMeetingDataCommandHandler : IRequestHandler<UpdateMeetingDataCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly IEmailSender _emailSender;

    public UpdateMeetingDataCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IUserContextService userContextService, IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
        _emailSender = emailSender;
    }

    public async Task<Unit> Handle(UpdateMeetingDataCommand request, CancellationToken cancellationToken)
    {
        var meeting = await _dbContext
                            .Meetings
                            .Include(x => x.Organizer)
                            .Include(x => x.MeetingParticipants).ThenInclude(x => x.Participant)
                            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (meeting is null) throw new AppException("Meeting not found.");

        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
            throw new AppException("User not found.");

        if (user.Role != Role.Administrator && user.Id != meeting.OrganizerId)
            throw new AppException("Only organizer or system admin can update meeting data.");

        var currentParticipantsQuantity = _dbContext.CountMeetingParticipantsQuantity(meeting.Id);
        if (currentParticipantsQuantity > request.MaxParticipantsQuantity)
            throw new AppException("New max participants quantity exceeds current participants count.");

        var youngestParticipantAge = meeting
            .MeetingParticipants
            .Where(x => x.InvitationStatus == InvitationStatus.Accepted)
            .Select(x => x.Participant.DateOfBirth.CalculateAge())
            .DefaultIfEmpty(int.MaxValue)
            .Min();
        if (youngestParticipantAge < request.MinParticipantsAge)
            throw new AppException("New min participants age below current youngest participant age.");

        _mapper.Map(request, meeting);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var participantsAccepted = meeting.MeetingParticipants.Where(x => x.InvitationStatus == InvitationStatus.Accepted).ToList();

        await SendEmails(meeting.Organizer, participantsAccepted, meeting.Title, meeting.Id);

        return await Task.FromResult(Unit.Value);
    }

    private async Task SendEmails(User organizer, List<MeetingParticipant> meetingParticipants, string meetingTitle, Guid meetingId)
    {
        var emailDto = Mails.GetUpdatedMeetingNotificationEmail(organizer.Email, organizer.Username, meetingTitle, meetingId);
        await _emailSender.SendEmailAsync(emailDto);

        foreach (var meetingParticipant in meetingParticipants)
        {
            emailDto = Mails.GetUpdatedMeetingNotificationEmail(meetingParticipant.Participant!.Email, meetingParticipant.Participant!.Username, meetingTitle, meetingId);
            await _emailSender.SendEmailAsync(emailDto);
        }
    }
}