using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Queries.MeetingPin.GetMeetingPinDetailsById;

public class GetMeetingPinDetailsByIdQuery : IRequest<MeetingPinDetailsDto>
{
    public Guid Id { get; set; }
}

public class GetAllMeetingPinsQueryHandler : IRequestHandler<GetMeetingPinDetailsByIdQuery, MeetingPinDetailsDto>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;
    public GetAllMeetingPinsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<MeetingPinDetailsDto> Handle(GetMeetingPinDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var meeting = _applicationDbContext.Meetings.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        var pinDetailsDto = _mapper.Map<MeetingPinDetailsDto>(meeting);

        return pinDetailsDto;
    }
}

