using Application.Common.Interfaces;
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

    public GetMeetingDetailsByIdQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<MeetingDetailsDto> Handle(GetMeetingDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var meetingDetails = _dbContext.Meetings.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        var meetingDetailsDto = _mapper.Map<MeetingDetailsDto>(meetingDetails);

        var organizer = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == meetingDetails.Result.OrganizerId, cancellationToken: cancellationToken);
        meetingDetailsDto.OrganizerUsername = organizer.Username;

        return meetingDetailsDto;
    }
}
