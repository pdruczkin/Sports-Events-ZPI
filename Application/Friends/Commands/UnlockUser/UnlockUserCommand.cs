using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Friends.Commands.UnlockUser;

public class UnlockUserCommand : IRequest<Unit>
{
    public Guid UserToUnlockId { get; set; }
}

public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public UnlockUserCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }

    public async Task<Unit> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");
        
        var userToUnlock = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == request.UserToUnlockId, cancellationToken);
        if (userToUnlock is null) throw new AppException("User to unlock is not found");
        
        var lastFriendShip = await _dbContext.Friendships.Where(x =>
                x.InviterId == userId && x.InviteeId == request.UserToUnlockId)
            .OrderByDescending(x => x.StatusDateTimeUtc)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (lastFriendShip is null || lastFriendShip.FriendshipStatus != FriendshipStatus.Blocked)
        {
            throw new AppException($"User {userToUnlock.Username} is already unlocked");
        }
        
        //To talk - what to do when unlocking in friendship dbSet ;)
        _dbContext.Friendships.Remove(lastFriendShip);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return await Task.FromResult(Unit.Value);
    }
} 
