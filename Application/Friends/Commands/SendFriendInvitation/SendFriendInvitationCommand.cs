using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Friends.Commands.SendFriendInvitation;

public class SendFriendInvitationCommand : IRequest<Unit>
{
    public string Username { get; set; }
}

public class SendFriendInvitationCommandHandler : IRequestHandler<SendFriendInvitationCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    public SendFriendInvitationCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    
    public async Task<Unit> Handle(SendFriendInvitationCommand request, CancellationToken cancellationToken)
    {
        var inviterId = _userContextService.GetUserId;

        var inviter = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == inviterId, cancellationToken);
        if (inviter is null) throw new AppException("User is not found");

        var invitee = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken);        
        if (invitee is null) throw new AppException("User is not found");

        var currentFriendshipState = await _dbContext
            .Friendships
            .Where(x => (x.Inviter == inviter && x.Invitee == invitee)
                    || (x.Inviter == invitee && x.Invitee == inviter))
            .OrderByDescending(x => x.StatusDateTimeUtc)
            .FirstOrDefaultAsync();

        if(currentFriendshipState != null)
        {
            if(currentFriendshipState.FriendshipStatus == FriendshipStatus.Invited)
                throw new AppException("User is alredy invited");

            if(currentFriendshipState.FriendshipStatus == FriendshipStatus.Accepted)
                throw new AppException("User is already a friend");

            if(currentFriendshipState.FriendshipStatus == FriendshipStatus.Blocked)
                throw new AppException("User is blocked");
        }
        
        _dbContext.Friendships.Add(new Friendship
        {
            Inviter = inviter,
            Invitee = invitee,
            FriendshipStatus = FriendshipStatus.Invited
        });
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await Task.FromResult(Unit.Value);
    }
}