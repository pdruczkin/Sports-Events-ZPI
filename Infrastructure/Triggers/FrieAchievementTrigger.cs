using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Triggers;

public class FrieAchievementTrigger : IAfterSaveTrigger<Friendship>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public FrieAchievementTrigger(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task AfterSave(ITriggerContext<Friendship> context, CancellationToken cancellationToken)
    {
        if (context.Entity.FriendshipStatus == FriendshipStatus.Accepted)
        {
            var inviterId = context.Entity.InviterId;
            var inviteeId = context.Entity.InviteeId;

            await CheckFrieAchievementAsync(inviterId, cancellationToken);
            await CheckFrieAchievementAsync(inviteeId, cancellationToken);
        }
        
        return;
    }

    private async Task CheckFrieAchievementAsync(Guid userId, CancellationToken cancellationToken)
    {
        var friendsCount = await FriendsCountAsync(userId, cancellationToken);
        await GrantFrieAchievementAsync(userId, friendsCount, cancellationToken);
    }

    private async Task<int> FriendsCountAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext
            .Users
            .Include(x => x.AsInvitee)
            .Include(x => x.AsInviter)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        var friendsAsInviteeCount = user
            .AsInvitee
            .GroupBy(x => x.InviterId)
            .Count(x => x.OrderByDescending(x => x.StatusDateTimeUtc).First().FriendshipStatus == FriendshipStatus.Accepted);

        var friendsAsInviterCount = user
            .AsInviter
            .GroupBy(x => x.InviteeId)
            .Count(x => x.OrderByDescending(x => x.StatusDateTimeUtc).First().FriendshipStatus == FriendshipStatus.Accepted);

        var totalFriendsCount = friendsAsInviteeCount + friendsAsInviterCount;

        return totalFriendsCount;
    }

    private async Task GrantFrieAchievementAsync(Guid userId, int friendsCount, CancellationToken cancellationToken)
    {
        if (friendsCount == 500)
        {
            await _dbContext.AddAchievementAsync(userId, "FRIE500", _dateTimeProvider, cancellationToken);
        }
        else if (friendsCount == 100)
        {
            await _dbContext.AddAchievementAsync(userId, "FRIE100", _dateTimeProvider, cancellationToken);
        }
        else if (friendsCount == 50)
        {
            await _dbContext.AddAchievementAsync(userId, "FRIE50", _dateTimeProvider, cancellationToken);
        }
        else if (friendsCount == 10)
        {
            await _dbContext.AddAchievementAsync(userId, "FRIE10", _dateTimeProvider, cancellationToken);
        }
        else if (friendsCount == 5)
        {
            await _dbContext.AddAchievementAsync(userId, "FRIE5", _dateTimeProvider, cancellationToken);
        }
        else if (friendsCount == 1)
        {
            await _dbContext.AddAchievementAsync(userId, "FRIE1", _dateTimeProvider, cancellationToken);
        }
    }
}
