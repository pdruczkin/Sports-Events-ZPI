using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Meetings.Queries.MeetingDetails.GetMeetingDetailsById;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Commands.CreateMeeting;

public class CreateMeetingCommand : IRequest<MeetingDetailsDto>, IMappable<Meeting>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; } // range: from -90 (South) to 90 (North)
    public double Longitude { get; set; } // range: from -180 (West) to 180 (East)
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime EndDateTimeUtc { get; set; }
    public MeetingVisibility Visibility { get; set; } = MeetingVisibility.Private;
    public SportsDiscipline SportsDiscipline { get; set; } = SportsDiscipline.Other;
    public Difficulty Difficulty { get; set; } = Difficulty.Amateur;
    public int MaxParticipantsQuantity { get; set; } = 100;
    public int MinParticipantsAge { get; set; } = 1;
}

public class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, MeetingDetailsDto>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public CreateMeetingCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, IUserContextService userContextService)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
        _userContextService = userContextService;
    }

    public async Task<MeetingDetailsDto> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) 
            throw new UnauthorizedException("Only registered users are allowed to arrange new meetings.");

        var newMeeting = _mapper.Map<Meeting>(request);
        newMeeting.OrganizerId = (Guid) userId;

        _applicationDbContext.Meetings.Add(newMeeting);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        var meetingDetailsDto = _mapper.Map<MeetingDetailsDto>(newMeeting);

        meetingDetailsDto.CurrentParticipantsQuantity = 1;
        meetingDetailsDto.IsOrganizer = true;
        meetingDetailsDto.Organizer = _mapper.Map<UserIdentityDto>(user);
        meetingDetailsDto.MeetingParticipants = new List<ParticipantIdentityDto>();

        return meetingDetailsDto;
    }
}
