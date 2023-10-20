using Application.Common.Exceptions;
using Application.Common.Interfaces;
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

    public GetFriendListQueryHandler(IApplicationDbContext applicationDbContext,IUserContextService userContextService)
    {
        _applicationDbContext = applicationDbContext;
        _userContextService = userContextService;
    }
    public async Task<List<FriendUsernameDto>> Handle(GetFriendsListQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;

        var user = await _applicationDbContext
            .Users
            .Include(x => x.AsInvitee)
                .ThenInclude(x => x.Inviter)
            .Include(x => x.AsInviter)
                .ThenInclude(x => x.Invitee)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var friendsAsInvitee = user
            .AsInvitee
            .GroupBy(x => x.Inviter)
            .Select(group => new
            {
                InviterUsername = group.Key.Username,
                InviterFirstName = group.Key.FirstName,
                InviterLastName = group.Key.LastName,
                CurrentFriendshipState = group.OrderByDescending(x => x.StatusDateTimeUtc).First()
            })
            .Where(x => x.CurrentFriendshipState.FriendshipStatus == FriendshipStatus.Accepted)
            .Select(x => new FriendUsernameDto
            {
                FriendUsername = x.InviterUsername,
                FirstName = x.InviterFirstName,
                LastName = x.InviterLastName
            });

        var friendsAsInviter = user
            .AsInviter
            .GroupBy(x => x.Invitee)
            .Select(group => new
            {
                InviteeUsername = group.Key.Username,
                InviteeFirstName = group.Key.FirstName,
                InviteeLastName = group.Key.LastName,
                CurrentFriendshipState = group.OrderByDescending(x => x.StatusDateTimeUtc).First()
            })
            .Where(x => x.CurrentFriendshipState.FriendshipStatus == FriendshipStatus.Accepted)
            .Select(x => new FriendUsernameDto
            {
                FriendUsername = x.InviteeUsername,
                FirstName = x.InviteeFirstName,
                LastName = x.InviteeLastName
            });

        var allFriends = friendsAsInvitee
            .Union(friendsAsInviter)
            .ToList();

        return allFriends;
    }
}
