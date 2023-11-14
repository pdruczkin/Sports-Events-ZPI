using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Achievements;

public class FrieAchievement
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public FrieAchievement(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task CheckFrieAchievementAsync(Guid userId, CancellationToken cancellationToken)
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
            await _dbContext.AddAchievementAsync(userId, "FRIE05", _dateTimeProvider, cancellationToken);
        }
        else if (friendsCount == 1)
        {
            await _dbContext.AddAchievementAsync(userId, "FRIE01", _dateTimeProvider, cancellationToken);
        }
    }



}
