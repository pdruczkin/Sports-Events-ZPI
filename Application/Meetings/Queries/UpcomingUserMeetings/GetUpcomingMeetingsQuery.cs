using Application.Common.Enums;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Meetings.Queries.UpcomingUserMeetings;

public class GetUpcomingMeetingsQuery : IRequest<PagedResult<UpcomingMeetingItemDto>>
{
    public DateTime? StartDateTimeUtcFrom { get; set; }
    public DateTime? StartDateTimeUtcTo { get; set; }
    public SportsDiscipline? SportsDiscipline { get; set; }
    public Difficulty? Difficulty { get; set; }
    public MeetingVisibility? MeetingVisibility { get; set; }
    public int? MaxParticipantsQuantity { get; set; }
    public int? CurrentParticipantsQuantityFrom { get; set; }
    public int? CurrentParticipantsQuantityTo { get; set; }
    public int? MinParticipantsAgeFrom { get; set; }
    public int? MinParticipantsAgeTo { get; set; }
    public string? TitleSearchPhrase { get; set; }
    public bool AsOrganizer { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection? SortDirection { get; set; }
}

public class GetMeetingsHistoryQueryHandler : IRequestHandler<GetUpcomingMeetingsQuery, PagedResult<UpcomingMeetingItemDto>>
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

    public async Task<PagedResult<UpcomingMeetingItemDto>> Handle(GetUpcomingMeetingsQuery request, CancellationToken cancellationToken)
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
                    { nameof(Meeting.StartDateTimeUtc), r => r.StartDateTimeUtc}
                };

            var selectedColumn = columnsSelectors[request.SortBy];

            filteredMeetingsBaseQuery = request.SortDirection == SortDirection.ASC
                ? filteredMeetingsBaseQuery.OrderBy(selectedColumn)
                : filteredMeetingsBaseQuery.OrderByDescending(selectedColumn);
        }

        var filteredMeetingsPaged = await GetPagedMeetings(filteredMeetingsBaseQuery, request.PageSize, request.PageNumber);

        var totalMeetingsCount = filteredMeetingsBaseQuery.Count();

        var meetingListItemsDtos = _mapper.Map<List<UpcomingMeetingItemDto>>(filteredMeetingsPaged);

        var pagedResult = new PagedResult<UpcomingMeetingItemDto>(meetingListItemsDtos, totalMeetingsCount, request.PageNumber, request.PageSize);

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

    private IQueryable<Meeting> GetFilteredMeetingsQuery(GetUpcomingMeetingsQuery request, User user)
    {
        var meetings = _applicationDbContext
                        .Meetings
                        .Include(x => x.Organizer)
                        .Include(x => x.MeetingParticipants);

        var filteredMeetingsIQueryable = meetings
                                    .Where(x => (request.StartDateTimeUtcFrom == null && x.StartDateTimeUtc >= DateTime.UtcNow) 
                                                    || x.StartDateTimeUtc >= request.StartDateTimeUtcFrom)
                                    .Where(x => (request.StartDateTimeUtcTo == null && x.StartDateTimeUtc >= DateTime.UtcNow)
                                                    || x.StartDateTimeUtc <= request.StartDateTimeUtcTo)
                                    .Where(x => request.SportsDiscipline == null || x.SportsDiscipline == request.SportsDiscipline)
                                    .Where(x => request.Difficulty == null || x.Difficulty == request.Difficulty)
                                    .Where(x => request.MeetingVisibility == null || x.Visibility == request.MeetingVisibility)
                                    .Where(x => request.MaxParticipantsQuantity == null || x.MaxParticipantsQuantity <= request.MaxParticipantsQuantity)
                                    .Where(x => request.CurrentParticipantsQuantityFrom == null || CountCurrentParticipantsQuantity(x.Id) >= request.CurrentParticipantsQuantityFrom)
                                    .Where(x => request.CurrentParticipantsQuantityTo == null || CountCurrentParticipantsQuantity(x.Id) <= request.CurrentParticipantsQuantityTo)
                                    .Where(x => request.MinParticipantsAgeFrom == null || x.MinParticipantsAge >= request.MinParticipantsAgeFrom)
                                    .Where(x => request.MinParticipantsAgeTo == null || x.MinParticipantsAge <= request.MinParticipantsAgeTo)
                                    .Where(x => request.TitleSearchPhrase == null || x.Title.ToLower().Contains(request.TitleSearchPhrase.ToLower()));

        if (request.AsOrganizer)
            filteredMeetingsIQueryable = filteredMeetingsIQueryable.Where(x => x.OrganizerId == user.Id);
        else
            filteredMeetingsIQueryable = filteredMeetingsIQueryable.Where(x => x.MeetingParticipants.Select(x => x.ParticipantId).Contains(user.Id));

        return filteredMeetingsIQueryable;
    }

    private int CountCurrentParticipantsQuantity(Guid meetingId)
    {
        var totalParticipantsQuantity = 1 + _applicationDbContext // 1 - meeting's organizer is also a participant
            .MeetingParticipants
            .Count(mp => mp.MeetingId == meetingId && mp.InvitationStatus == InvitationStatus.Accepted);
        return totalParticipantsQuantity;
    }
}