using Application.Common.Exceptions;
using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Queries.MeetingDetails.GetMeetingDetailsById;

public class GetMeetingDetailsByIdQuery : IRequest<MeetingDetailsDto>
{
    public Guid Id { get; set; }
}

public class GetMeetingDetailsByIdQueryHandler : IRequestHandler<GetMeetingDetailsByIdQuery, MeetingDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public GetMeetingDetailsByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
    }

    public async Task<MeetingDetailsDto> Handle(GetMeetingDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var meetingDetails = await _dbContext
                            .Meetings
                            .Include(x => x.Organizer)
                            .Include(x => x.MeetingParticipants).ThenInclude(x => x.Participant)
                            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        
        if(meetingDetails == null)
            throw new AppException("Meeting not found.");

        var meetingDetailsDto = _mapper.Map<MeetingDetailsDto>(meetingDetails);

        meetingDetailsDto.CurrentParticipantsQuantity = meetingDetails.CountMeetingParticipantsQuantity();

        var userId = _userContextService.GetUserId;

        // flag if the user from token is also the meeting's organizer
        meetingDetailsDto.IsOrganizer = (userId != null && userId == meetingDetails.OrganizerId);

        var participants = meetingDetails.MeetingParticipants.Select(x => new ParticipantIdentityDto
            { Id = x.Participant.Id, Username = x.Participant.Username, Status = x.InvitationStatus }).ToList();

        meetingDetailsDto.MeetingParticipants = participants;
        
        return meetingDetailsDto;
    }
}
