using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Meetings.Queries.MeetingDetails.GetMeetingDetailsById;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.IO.IsolatedStorage;

namespace Application.Friends.Queries.GetFriendInvitations;

public class GetFriendInvitationsQuery : IRequest<List<FriendInvitationsDto>>
{

}

public class GetFriendInvitationsQueryHandler : IRequestHandler<GetFriendInvitationsQuery, List<FriendInvitationsDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;

    public GetFriendInvitationsQueryHandler(IApplicationDbContext applicationDbContext, IUserContextService userContextService, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _userContextService = userContextService;
        _mapper = mapper;
    }
    public async Task<List<FriendInvitationsDto>> Handle(GetFriendInvitationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;

        var user = await _applicationDbContext
            .Users
            .Include(x => x.AsInvitee)
            .ThenInclude(x => x.Inviter).ThenInclude(x => x.Image)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var friendInvitationsDtos = user
            .AsInvitee
            .GroupBy(x => x.Inviter)
            .Select(group => new
            {
                InviterId = group.Key.Id,
                InviterUsername = group.Key.Username,
                CurrentFriendshipState = group.OrderByDescending(x => x.StatusDateTimeUtc).First(),
                InviterImage = group.Key.Image
            })
            .Where(x => x.CurrentFriendshipState.FriendshipStatus == FriendshipStatus.Invited)
            .Select(x => new FriendInvitationsDto
            {
                InviterId = x.InviterId,
                InviterUsername = x.InviterUsername,
                InvitationDateTimeUtc = (x.CurrentFriendshipState.StatusDateTimeUtc),
                Image = _mapper.Map<ImageDto>(x.InviterImage)
            })
            .ToList();

        return friendInvitationsDtos;
    }
}