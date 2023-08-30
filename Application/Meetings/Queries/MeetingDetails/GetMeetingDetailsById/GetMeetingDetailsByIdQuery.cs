using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
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
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Meeting, MeetingDetailsDto>()
            .ForMember(dto => dto.OrganizerUsername, o => o.MapFrom(m => m.Organizer.Username));
    }

    public async Task<MeetingDetailsDto> Handle(GetMeetingDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var meetingDetails = await _dbContext
                            .Meetings
                            .Include(x => x.Organizer)
                            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        var meetingDetailsDto = _mapper.Map<MeetingDetailsDto>(meetingDetails);

        return meetingDetailsDto;
    }
}
