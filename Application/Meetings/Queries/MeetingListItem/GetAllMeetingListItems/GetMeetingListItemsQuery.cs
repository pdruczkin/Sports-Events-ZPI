using Application.Common.Enums;
using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;

public class GetMeetingListItemsQuery : IRequest<List<MeetingListItemDto>>
{
    public double SouthWestLatitude { get; set; }
    public double SouthWestLongitude { get; set; }
    public double NorthEastLatitude { get; set; }
    public double NorthEastLongitude { get; set; }
    public DateTime? StartDateTimeUtc { get; set; }
    public SportsDiscipline? SportsDiscipline { get; set; }
    public Difficulty? Difficulty { get; set; }
    public int? MaxParticipantsQuantity { get; set; }
    public int? CurrentParticipantsQuantity { get; set; }
    public int? MinParticipantsAge { get; set; }
    public string? TitleSearchPhrase { get; set; }
    public string? SortBy { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.ASC;
}

public class GetAllMeetingListItemsQueryHandler : IRequestHandler<GetMeetingListItemsQuery, List<MeetingListItemDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public GetAllMeetingListItemsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, IUserContextService userContextService)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
        _userContextService = userContextService;
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Meeting, MeetingListItemDto>()
            .ForMember(dto => dto.OrganizerUsername, o => o.MapFrom(m => m.Organizer.Username));
    }

    public async Task<List<MeetingListItemDto>> Handle(GetMeetingListItemsQuery request, CancellationToken cancellationToken)
    {
        var filteredMeetingsBaseQuery = GetFilteredMeetingsQuery(request);

        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var columnsSelectors = new Dictionary<string, Expression<Func<Meeting, object>>>
                {
                    { nameof(Meeting.StartDateTimeUtc), r => r.StartDateTimeUtc },
                    { nameof(Meeting.Difficulty), r => r.Difficulty },
                    { nameof(Meeting.MaxParticipantsQuantity), r => r.MaxParticipantsQuantity }
                };

            var selectedColumn = columnsSelectors[request.SortBy];

            filteredMeetingsBaseQuery = request.SortDirection == SortDirection.ASC
                ? filteredMeetingsBaseQuery.OrderBy(selectedColumn)
                : filteredMeetingsBaseQuery.OrderByDescending(selectedColumn);
        }

        var filteredMeetings = await filteredMeetingsBaseQuery.ToListAsync(cancellationToken);

        var meetingListItemsDtos = _mapper.Map<List<MeetingListItemDto>>(filteredMeetings);

        meetingListItemsDtos.ForEach(x => x.CurrentParticipantsQuantity = _applicationDbContext.CountMeetingParticipantsQuantity(x.Id));

        return meetingListItemsDtos;
    }

    private IQueryable<Meeting> GetFilteredMeetingsQuery(GetMeetingListItemsQuery request)
    {
        var userId = _userContextService.GetUserId;

        var meetings = _applicationDbContext
                        .Meetings
                        .Include(x => x.Organizer)
                        .Include(x => x.MeetingParticipants);

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
                                    .Where(x => request.MaxParticipantsQuantity == null || x.MaxParticipantsQuantity <= request.MaxParticipantsQuantity)
                                    .Where(x => request.CurrentParticipantsQuantity == null || x.CountMeetingParticipantsQuantity() <= request.CurrentParticipantsQuantity)
                                    .Where(x => request.MinParticipantsAge == null || x.MinParticipantsAge >= request.MinParticipantsAge)
                                    .Where(x => request.TitleSearchPhrase == null || x.Title.ToLower().Contains(request.TitleSearchPhrase.ToLower()))
                                    .Where(x => x.Visibility == MeetingVisibility.Public || 
                                            (userId != null && (x.OrganizerId == userId || x.MeetingParticipants.Any(x => x.ParticipantId == userId && x.InvitationStatus != InvitationStatus.Rejected))));

        return filteredMeetingsIQueryable;
    }
}