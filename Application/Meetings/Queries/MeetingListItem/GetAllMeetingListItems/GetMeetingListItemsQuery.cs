using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;

public class GetMeetingListItemsQuery : IRequest<PagedResult<MeetingListItemDto>>
{
    public double SouthWestLatitude { get; set; }
    public double SouthWestLongitude { get; set; }
    public double NorthEastLatitude { get; set; }
    public double NorthEastLongitude { get; set; }
    public DateTime? StartDateTimeUtc { get; set; }
    public SportsDiscipline? SportsDiscipline { get; set; }
    public Difficulty? Difficulty { get; set; }
    public MeetingVisibility? MeetingVisibility { get; set; }
    public string? TitleSearchPhrase { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetAllMeetingListItemsQueryHandler : IRequestHandler<GetMeetingListItemsQuery, PagedResult<MeetingListItemDto>>
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

    public async Task<PagedResult<MeetingListItemDto>> Handle(GetMeetingListItemsQuery request, CancellationToken cancellationToken)
    {
        var filteredMeetingsBaseQuery = GetFilteredMeetingsQuery(request);
        var filteredMeetingsPaged = await GetPagedMeetings(filteredMeetingsBaseQuery, request.PageSize, request.PageNumber);

        var totalMeetingsCount = filteredMeetingsBaseQuery.Count();

        var meetingListItemsDtos = _mapper.Map<List<MeetingListItemDto>>(filteredMeetingsPaged);

        var pagedResult = new PagedResult<MeetingListItemDto>(meetingListItemsDtos, totalMeetingsCount, request.PageNumber, request.PageSize);

        return pagedResult;
    }

    private async Task<List<Meeting>> GetPagedMeetings(IQueryable<Meeting> query, int pageSize, int pageNumber)
    {
        var pagedMeetings = await query
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return pagedMeetings;
    }

    private IQueryable<Meeting> GetFilteredMeetingsQuery(GetMeetingListItemsQuery request)
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
                                    .Where(x => request.MeetingVisibility == null || x.Visibility == request.MeetingVisibility)
                                    .Where(x => request.TitleSearchPhrase == null || x.Title.ToLower().Contains(request.TitleSearchPhrase.ToLower()));

        return filteredMeetingsIQueryable;
    }

}