using Application.Common.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;

public class GetAllMeetingListItemsQuery : IRequest<IEnumerable<MeetingListItemDto>>
{

}

public class GetAllMeetingListItemsQueryHandler : IRequestHandler<GetAllMeetingListItemsQuery, IEnumerable<MeetingListItemDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public GetAllMeetingListItemsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MeetingListItemDto>> Handle(GetAllMeetingListItemsQuery request, CancellationToken cancellationToken)
    {
        var meetingListItemsDtos = _mapper.Map<IEnumerable<MeetingListItemDto>>(_applicationDbContext.Meetings);

        return meetingListItemsDtos;
    }
}

