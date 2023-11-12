using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Domain.Entities;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

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
            var organizerId = context.Entity.OrganizerId;

            var organizedMeetingsCount = await _dbContext
                .Meetings
                .CountAsync(x => x.OrganizerId == organizerId, cancellationToken);

            if (organizedMeetingsCount == 50)
            {
                await _dbContext.AddAchievementAsync(organizerId, "ORGN50", _dateTimeProvider, cancellationToken);
            }
            else if (organizedMeetingsCount == 10)
            {
                await _dbContext.AddAchievementAsync(organizerId, "ORGN10", _dateTimeProvider, cancellationToken);
            }
            else if (organizedMeetingsCount == 5)
            {
                await _dbContext.AddAchievementAsync(organizerId, "ORGN5", _dateTimeProvider, cancellationToken);
            }
            else if (organizedMeetingsCount == 1)
            {
                await _dbContext.AddAchievementAsync(organizerId, "ORGN1", _dateTimeProvider, cancellationToken);
            }
        }

        return;
    }
}