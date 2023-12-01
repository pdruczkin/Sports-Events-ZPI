using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UserDetails.Queries.GetUserDetails;

public class GetUserDetailsCommand : IRequest<UserDetails>
{
    
}

public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsCommand, UserDetails>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetUserDetailsQueryHandler(IApplicationDbContext dbContext, IMapper mapper, IUserContextService userContextService, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
        _dateTimeProvider = dateTimeProvider;
    }
    
    public async Task<UserDetails> Handle(GetUserDetailsCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .Include(x => x.Image)
            .Include(x => x.MeetingParticipants).ThenInclude(x => x.Meeting).ThenInclude(x => x.Organizer)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user is null) throw new AppException("User is not found");

        var userDetails = _mapper.Map<UserDetails>(user);
        
        var recentMeetings = user.MeetingParticipants
            .Where(x => x.InvitationStatus == InvitationStatus.Accepted)
            .Where(x => x.Meeting!.EndDateTimeUtc < _dateTimeProvider.UtcNow)
            .OrderByDescending(x => x.Meeting!.StartDateTimeUtc)
            .Take(1)
            .Select(x => x.Meeting)
            .ToList();

        userDetails.RecentMeetings = _mapper.Map<List<MeetingPinDto>>(recentMeetings);
        userDetails.HasAdminRole = _userContextService.HasAdminRole();
        
        return userDetails;
    }
}



