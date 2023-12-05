using Application.Common.Enums;
using Application.Common.Exceptions;
using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Meetings.Queries.MeetingHistory;

public class GetMeetingsHistoryQuery : IRequest<PagedResult<MeetingHistoryItemDto>>
{
    public DateTime? StartDateTimeUtcFrom { get; set; }
    public DateTime? StartDateTimeUtcTo { get; set; }
    public SportsDiscipline? SportsDiscipline { get; set; }
    public Difficulty? Difficulty { get; set; }
    public MeetingVisibility? MeetingVisibility { get; set; }
    public int? MaxParticipantsQuantity { get; set; }
    public int? FinalParticipantsQuantityFrom { get; set; }
    public int? FinalParticipantsQuantityTo { get; set; }
    public int? MinParticipantsAgeFrom { get; set; }
    public int? MinParticipantsAgeTo { get; set; }
    public string? TitleSearchPhrase { get; set; }
    public bool AsOrganizer { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.ASC;
}

public class GetMeetingsHistoryQueryHandler : IRequestHandler<GetMeetingsHistoryQuery, PagedResult<MeetingHistoryItemDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public GetMeetingsHistoryQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, IUserContextService userContextService)
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

    public async Task<PagedResult<MeetingHistoryItemDto>> Handle(GetMeetingsHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var filteredMeetingsBaseQuery = GetFilteredMeetingsQuery(request, user);

        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var columnsSelectors = new Dictionary<string, Expression<Func<Meeting, object>>>
                {
                    { nameof(Meeting.StartDateTimeUtc), r => r.StartDateTimeUtc },
                    { nameof(Meeting.Difficulty), r => r.Difficulty },
                    { nameof(Meeting.MaxParticipantsQuantity), r => r.MaxParticipantsQuantity },
                };

            var selectedColumn = columnsSelectors[request.SortBy];

            filteredMeetingsBaseQuery = request.SortDirection == SortDirection.ASC
                ? filteredMeetingsBaseQuery.OrderBy(selectedColumn)
                : filteredMeetingsBaseQuery.OrderByDescending(selectedColumn);
        }

        var filteredMeetingsPaged = await GetPagedMeetings(filteredMeetingsBaseQuery, request.PageSize, request.PageNumber);

        var totalMeetingsCount = filteredMeetingsBaseQuery.Count();

        var meetingListItemsDtos = _mapper.Map<List<MeetingHistoryItemDto>>(filteredMeetingsPaged);
        
        meetingListItemsDtos.ForEach(x => x.FinalParticipantsQuantity = _applicationDbContext.CountMeetingParticipantsQuantity(x.Id));

        var pagedResult = new PagedResult<MeetingHistoryItemDto>(meetingListItemsDtos, totalMeetingsCount, request.PageNumber, request.PageSize);

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

    private IQueryable<Meeting> GetFilteredMeetingsQuery(GetMeetingsHistoryQuery request, User user)
    {
        var meetings = _applicationDbContext
                        .Meetings
                        .Include(x => x.Organizer)
                        .Include(x => x.MeetingParticipants);

        var filteredMeetingsIQueryable = meetings
                                    .Where(x => (request.StartDateTimeUtcFrom == null && x.StartDateTimeUtc < DateTime.UtcNow) 
                                                    || x.StartDateTimeUtc >= request.StartDateTimeUtcFrom)
                                    .Where(x => (request.StartDateTimeUtcTo == null && x.StartDateTimeUtc < DateTime.UtcNow)
                                                    || x.StartDateTimeUtc <= request.StartDateTimeUtcTo)
                                    .Where(x => request.SportsDiscipline == null || x.SportsDiscipline == request.SportsDiscipline)
                                    .Where(x => request.Difficulty == null || x.Difficulty == request.Difficulty)
                                    .Where(x => request.MeetingVisibility == null || x.Visibility == request.MeetingVisibility)
                                    .Where(x => request.MaxParticipantsQuantity == null || x.MaxParticipantsQuantity <= request.MaxParticipantsQuantity)
                                    .Where(x => request.FinalParticipantsQuantityFrom == null || x.MeetingParticipants.Count(mp => mp.MeetingId == x.Id && mp.InvitationStatus == InvitationStatus.Accepted) + 1 >= request.FinalParticipantsQuantityFrom)
                                    .Where(x => request.FinalParticipantsQuantityTo == null || x.MeetingParticipants.Count(mp => mp.MeetingId == x.Id && mp.InvitationStatus == InvitationStatus.Accepted) + 1 <= request.FinalParticipantsQuantityTo)
                                    .Where(x => request.MinParticipantsAgeFrom == null || x.MinParticipantsAge >= request.MinParticipantsAgeFrom)
                                    .Where(x => request.MinParticipantsAgeTo == null || x.MinParticipantsAge <= request.MinParticipantsAgeTo)
                                    .Where(x => request.TitleSearchPhrase == null || x.Title.ToLower().Contains(request.TitleSearchPhrase.ToLower()));

        if (request.AsOrganizer)
            filteredMeetingsIQueryable = filteredMeetingsIQueryable.Where(x => x.OrganizerId == user.Id);
        else
            filteredMeetingsIQueryable = filteredMeetingsIQueryable.Where(x => x.MeetingParticipants.Select(x => x.ParticipantId).Contains(user.Id));

        return filteredMeetingsIQueryable;
    }
}