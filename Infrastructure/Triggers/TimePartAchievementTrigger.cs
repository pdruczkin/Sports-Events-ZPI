using Application.Achievements;
using Application.Common.Interfaces;
using Domain.Entities;
using EntityFrameworkCore.Triggered;
using Hangfire;

namespace Infrastructure.Triggers;

public class TimePartAchievementTrigger : IAfterSaveTrigger<Meeting>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    public TimePartAchievementTrigger(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }
    public async Task AfterSave(ITriggerContext<Meeting> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Added)
        {
            BackgroundJob.Schedule<PartAchievement>(x => x.CheckPartAchievement(context.Entity.Id, cancellationToken),
                context.Entity.EndDateTimeUtc - _dateTimeProvider.UtcNow);
        }
    }
}