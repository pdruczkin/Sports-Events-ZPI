using Application.Account.Commands.RegisterUser;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Meeting, MeetingListItemDto>()
            .ForMember(dto => dto.OrganizerUsername, o => o.MapFrom(m => m.Organizer.Username));
    }

    public async Task<IEnumerable<MeetingListItemDto>> Handle(GetAllMeetingListItemsQuery request, CancellationToken cancellationToken)
    {
        var meetings = _applicationDbContext
                        .Meetings
                        .Include(nameof(Meeting.Organizer));

        var meetingListItemsDtos = _mapper.Map<IEnumerable<MeetingListItemDto>>(meetings);

        return meetingListItemsDtos;
    }
}