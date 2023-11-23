using Application.Achievements;
using Application.Common.Interfaces;
using Domain.Entities;
using EntityFrameworkCore.Triggered;

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
        if (context.ChangeType == ChangeType.Added)
        {
            var orgnAchievementChecker = new FrieAchievement(_dbContext, _dateTimeProvider);
            await orgnAchievementChecker.CheckFrieAchievementAsync(context.Entity.InviterId, cancellationToken);
            await orgnAchievementChecker.CheckFrieAchievementAsync(context.Entity.InviteeId, cancellationToken);
        }
    }
}
