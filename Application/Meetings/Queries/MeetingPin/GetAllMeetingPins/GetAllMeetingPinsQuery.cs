using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using MediatR;

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
        var allPinsDtos = _mapper.Map<IEnumerable<MeetingPinDto>>(_applicationDbContext.Meetings);

        return allPinsDtos;
    }
}

