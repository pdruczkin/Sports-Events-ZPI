using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Friends.Commands.RemoveFriend;

public class RemoveFriendCommand : IRequest<Unit>
{
    public Guid FriendToRemoveId { get; set; }
}

public class RemoveFriendCommandHandler : IRequestHandler<RemoveFriendCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public RemoveFriendCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }


    public async Task<Unit> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .Include(x => x.AsInviter)
            .Include(x => x.AsInvitee)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var friendToDelete = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == request.FriendToRemoveId, cancellationToken);
        if (friendToDelete is null) throw new AppException("User is not found");

        var userFriendships = user
            .AsInvitee
            .Union(user.AsInviter)
            .ToList();

        var currentFriendshipState = userFriendships
            .Where(x => (x.InviterId == userId && x.InviteeId == request.FriendToRemoveId)
                    || (x.InviterId == request.FriendToRemoveId && x.InviteeId == userId))
            .OrderByDescending(x => x.StatusDateTimeUtc)
            .FirstOrDefault();

        if (currentFriendshipState is null)
            throw new AppException("No relationship found between users");

        if (currentFriendshipState.FriendshipStatus != FriendshipStatus.Accepted)
            throw new AppException("Removing user from friends list not available for current friendship status");

        currentFriendshipState.FriendshipStatus = FriendshipStatus.Rejected;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }
}
