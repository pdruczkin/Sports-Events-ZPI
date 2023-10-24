using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Application.Friends.Commands.BlockUser;

public class BlockUserCommand : IRequest<Unit>
{
    public Guid UserToBlockId { get; set; }
}

public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public BlockUserCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    
    
    public async Task<Unit> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");
        
        var userToBlock = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == request.UserToBlockId, cancellationToken);
        if (userToBlock is null) throw new AppException("User to block is not found");

        if(userId == userToBlock.Id) throw new AppException("You're not allowed to block yourself" );
        
        var lastFriendShip = await _dbContext.Friendships.Where(x =>
            x.InviterId == userId && x.InviteeId == request.UserToBlockId)
            .OrderByDescending(x => x.StatusDateTimeUtc)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (lastFriendShip is not null && lastFriendShip.FriendshipStatus == FriendshipStatus.Blocked)
        {
            throw new AppException($"User {userToBlock.Username} is already blocked");
        }
        
        var blockedFriendship = new Friendship
        {
            Inviter = user,
            Invitee = userToBlock,
            FriendshipStatus = FriendshipStatus.Blocked
        };
        _dbContext.Friendships.Add(blockedFriendship);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return await Task.FromResult(Unit.Value);
    }
}
