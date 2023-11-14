using Application.Achievements;
using Application.Common.Interfaces;
using Domain.Entities;
using EntityFrameworkCore.Triggered;

namespace Infrastructure.Triggers;

public class OrgnAchievementTrigger : IAfterSaveTrigger<Meeting>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OrgnAchievementTrigger(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task AfterSave(ITriggerContext<Meeting> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Added)
        {
            var orgnAchievementChecker = new OrgnAchievement(_dbContext, _dateTimeProvider);
            await orgnAchievementChecker.CheckOrgnAchievementAsync(context.Entity.Id, cancellationToken);
        }
    }
}