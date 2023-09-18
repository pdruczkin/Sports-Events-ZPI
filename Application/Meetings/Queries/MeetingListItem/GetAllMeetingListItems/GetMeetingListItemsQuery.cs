using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;

public class GetMeetingListItemsQuery : IRequest<IEnumerable<MeetingListItemDto>>
{
    public double SouthWestLatitude { get; set; }
    public double SouthWestLongitude { get; set; }
    public double NorthEastLatitude { get; set; }
    public double NorthEastLongitude { get; set; }
    public DateTime? StartDateTimeUtc { get; set; }
    public SportsDiscipline? SportsDiscipline { get; set; }
    public Difficulty? Difficulty { get; set; }
    public MeetingVisibility? MeetingVisibility { get; set; }
}

public class GetAllMeetingListItemsQueryHandler : IRequestHandler<GetMeetingListItemsQuery, IEnumerable<MeetingListItemDto>>
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

    public async Task<IEnumerable<MeetingListItemDto>> Handle(GetMeetingListItemsQuery request, CancellationToken cancellationToken)
    {
        var filteredMeetings = await GetFilteredMeetings(request);

        var meetingListItemsDtos = _mapper.Map<IEnumerable<MeetingListItemDto>>(filteredMeetings);

        return meetingListItemsDtos;
    }

    private async Task<List<Meeting>> GetFilteredMeetings(GetMeetingListItemsQuery request)
    {
        var meetings = _applicationDbContext
                        .Meetings
                        .Include(x => x.Organizer);

        IQueryable<Meeting> filteredMeetingsIQueryable;        

        if (request.SouthWestLongitude < request.NorthEastLongitude) // "typical" situation
        {
            filteredMeetingsIQueryable = meetings
                                        .Where(x => x.Latitude >= request.SouthWestLatitude &&
                                                    x.Latitude <= request.NorthEastLatitude &&
                                                    x.Longitude >= request.SouthWestLongitude &&
                                                    x.Longitude <= request.NorthEastLongitude);

        }
        else // when longitude turns 180 E -> -180 W
        {
            filteredMeetingsIQueryable = meetings
                                        .Where(x => x.Latitude >= request.SouthWestLatitude &&
                                                    x.Latitude <= request.NorthEastLatitude &&
                                                    (x.Longitude >= request.SouthWestLongitude ||
                                                     x.Longitude <= request.NorthEastLongitude));
        }

        filteredMeetingsIQueryable = filteredMeetingsIQueryable
                                    .Where(x => (request.StartDateTimeUtc == null && x.StartDateTimeUtc > DateTime.UtcNow) || x.StartDateTimeUtc > request.StartDateTimeUtc)
                                    .Where(x => request.SportsDiscipline == null || x.SportsDiscipline == request.SportsDiscipline)
                                    .Where(x => request.Difficulty == null || x.Difficulty == request.Difficulty)
                                    .Where(x => request.MeetingVisibility == null || x.Visibility == request.MeetingVisibility);

        return await filteredMeetingsIQueryable.ToListAsync();
    }

}