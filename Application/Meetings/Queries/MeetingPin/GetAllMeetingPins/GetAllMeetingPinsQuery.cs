using Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Queries.MeetingPin.GetAllMeetingPins;

public class GetAllMeetingPinsQuery : IRequest<IEnumerable<MeetingPinDto>>
{

}

public class GetAllMeetingPinsQueryHandler : IRequestHandler<GetAllMeetingPinsQuery, IEnumerable<MeetingPinDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;
    public GetAllMeetingPinsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MeetingPinDto>> Handle(GetAllMeetingPinsQuery request, CancellationToken cancellationToken)
    {
        var meetings = await _applicationDbContext.Meetings.ToListAsync();
        var allPinsDtos = _mapper.Map<IEnumerable<MeetingPinDto>>(meetings);

        return allPinsDtos;
    }
}

