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
    private readonly IDateTimeProvider _dateTimeProvider;
    public SendFriendInvitationCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _dateTimeProvider = dateTimeProvider;
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

        var currentFrienshipStatus = await _applicationDbContext
            .Frienships
            .Where((x => x.Inviter == inviter && x.Invitee == invitee)
                    || (x => x.Inviter == invitee && x.Invitee == inviter))
            .OrderByDescending(x => x.StatusDateTimeUtc)
            .FirstOrDefaultAsync();

        if(currentFrienshipStatus == FriendshipStatus.Invited)
            throw new AppException("User is alredy invited");

        if(currentFrienshipStatus == FriendshipStatus.Accepted)
            throw new AppException("User is already a friend");

        if(frienshipHistory.FirstOrDefault(x => x.FriendshipStatus == FriendshipStatus.Blocked))
            throw new AppException("User is blocked");
        
        _applicationDbContext.Friendships.Add(new Friendship
        {
            Inviter = inviter,
            Invitee = invitee,
            FriendshipStatus = FriendshipStatus.Invited
        });
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await Task.FromResult(Unit.Value);
    }
}