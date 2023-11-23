using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Friends.Queries.GetFriendsList;

public class GetFriendsListQuery : IRequest<List<FriendUsernameDto>>
{

}

public class GetFriendListQueryHandler : IRequestHandler<GetFriendsListQuery, List<FriendUsernameDto>>
{
    public IApplicationDbContext _applicationDbContext { get; set; }
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;

    public GetFriendListQueryHandler(IApplicationDbContext applicationDbContext, IUserContextService userContextService, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _userContextService = userContextService;
        _mapper = mapper;
    }
    public async Task<List<FriendUsernameDto>> Handle(GetFriendsListQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;

        var user = await _applicationDbContext
            .Users
            .Include(x => x.AsInvitee)
                .ThenInclude(x => x.Inviter)
                .ThenInclude(x => x.Image)
            .Include(x => x.AsInviter)
                .ThenInclude(x => x.Invitee)
                .ThenInclude(x => x.Image)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var friendsAsInvitee = user
            .AsInvitee
            .GroupBy(x => x.Inviter)
            .Select(group => new
            {
                InviterId = group.Key.Id,
                InviterUsername = group.Key.Username,
                InviterFirstName = group.Key.FirstName,
                InviterLastName = group.Key.LastName,
                InviterImage = group.Key.Image,
                CurrentFriendshipState = group.OrderByDescending(x => x.StatusDateTimeUtc).First()
            })
            .Where(x => x.CurrentFriendshipState.FriendshipStatus == FriendshipStatus.Accepted)
            .Select(x => new FriendUsernameDto
            {
                Id = x.InviterId,
                FriendUsername = x.InviterUsername,
                FirstName = x.InviterFirstName,
                LastName = x.InviterLastName,
                Image = _mapper.Map<ImageDto>(x.InviterImage)
            });

        var friendsAsInviter = user
            .AsInviter
            .GroupBy(x => x.Invitee)
            .Select(group => new
            {
                InviteId = group.Key.Id,
                InviteeUsername = group.Key.Username,
                InviteeFirstName = group.Key.FirstName,
                InviteeLastName = group.Key.LastName,
                InviteeImage = group.Key.Image,
                CurrentFriendshipState = group.OrderByDescending(x => x.StatusDateTimeUtc).First()
            })
            .Where(x => x.CurrentFriendshipState.FriendshipStatus == FriendshipStatus.Accepted)
            .Select(x => new FriendUsernameDto
            {
                Id = x.InviteId,
                FriendUsername = x.InviteeUsername,
                FirstName = x.InviteeFirstName,
                LastName = x.InviteeLastName,
                Image = _mapper.Map<ImageDto>(x.InviteeImage)
            });

        var allFriends = friendsAsInvitee
            .Union(friendsAsInviter)
            .ToList();

        return allFriends;
    }
}
