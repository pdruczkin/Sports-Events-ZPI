using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Meetings.Queries.GetMeetingsInvitations;

public class GetMeetingsInvitationsQuery : IRequest<List<MeetingInvitationsDto>>
{
    
}

public class
    GetMeetingsInvitationsQueryHandler : IRequestHandler<GetMeetingsInvitationsQuery, List<MeetingInvitationsDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetMeetingsInvitationsQueryHandler(IApplicationDbContext dbContext, IMapper mapper,
        IUserContextService userContextService, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<List<MeetingInvitationsDto>> Handle(GetMeetingsInvitationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var meetingParticipants = await _dbContext
            .Meetings
            .Include(x => x.MeetingParticipants)
            .Where(x => x.StartDateTimeUtc > _dateTimeProvider.UtcNow)
            .Where(x => x.MeetingParticipants.Any(mp =>
                mp.ParticipantId == userId && mp.InvitationStatus == InvitationStatus.Pending))
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<MeetingInvitationsDto>>(meetingParticipants);
    }
}