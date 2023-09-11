using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;

public class GetAllMeetingListItemsQuery : IRequest<IEnumerable<MeetingListItemDto>>
{
    public double SouthWestLatitude { get; set; }
    public double SouthWestLongitude { get; set; }
    public double NorthEastLatitude { get; set; }
    public double NorthEastLongitude { get; set; }
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

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Meeting, MeetingListItemDto>()
            .ForMember(dto => dto.OrganizerUsername, o => o.MapFrom(m => m.Organizer.Username));
    }

    public async Task<IEnumerable<MeetingListItemDto>> Handle(GetAllMeetingListItemsQuery request, CancellationToken cancellationToken)
    {
        var meetings = await GetMeetingsInLngLatBounds(request);

        var meetingListItemsDtos = _mapper.Map<IEnumerable<MeetingListItemDto>>(meetings);

        return meetingListItemsDtos;
    }

    private async Task<List<Meeting>> GetMeetingsInLngLatBounds(GetAllMeetingListItemsQuery lngLatBounds)
    {
        if (lngLatBounds.SouthWestLongitude < lngLatBounds.NorthEastLongitude) // "typical" situation
        {
            return await _applicationDbContext
                    .Meetings
                    .Where(x => x.Latitude >= lngLatBounds.SouthWestLatitude &&
                                x.Latitude <= lngLatBounds.NorthEastLatitude &&
                                x.Longitude >= lngLatBounds.SouthWestLongitude &&
                                x.Longitude <= lngLatBounds.NorthEastLongitude
                          )
                    .ToListAsync();
        }
        else // when longitude turns 180 E -> -180 W
        {
            return await _applicationDbContext
                    .Meetings
                    .Where(x => x.Latitude >= lngLatBounds.SouthWestLatitude &&
                                x.Latitude <= lngLatBounds.NorthEastLatitude &&
                                (x.Longitude >= lngLatBounds.SouthWestLongitude ||
                                 x.Longitude <= lngLatBounds.NorthEastLongitude)
                          )
                    .ToListAsync();
        }
    }
}