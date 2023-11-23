using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Achievements;

public class OrgnAchievement
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OrgnAchievement(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task CheckOrgnAchievementAsync(Guid meetingId, CancellationToken cancellationToken)
    {
        var organizerId = await _dbContext
            .Meetings
            .Where(x => x.Id == meetingId)
            .Select(x => x.OrganizerId)
            .FirstOrDefaultAsync();

        var organizedMeetingsCount = await _dbContext
            .Meetings
            .CountAsync(x => x.OrganizerId == organizerId, cancellationToken);

        await GrantOrgnAchievementAsync(organizerId, organizedMeetingsCount, cancellationToken);
    }

    private async Task GrantOrgnAchievementAsync(Guid userId, int organizedMeetingsCount, CancellationToken cancellationToken)
    {
        if (organizedMeetingsCount == 50)
        {
            await _dbContext.AddAchievementAsync(userId, "ORGN50", _dateTimeProvider, cancellationToken);
        }
        else if (organizedMeetingsCount == 10)
        {
            await _dbContext.AddAchievementAsync(userId, "ORGN10", _dateTimeProvider, cancellationToken);
        }
        else if (organizedMeetingsCount == 5)
        {
            await _dbContext.AddAchievementAsync(userId, "ORGN05", _dateTimeProvider, cancellationToken);
        }
        else if (organizedMeetingsCount == 1)
        {
            await _dbContext.AddAchievementAsync(userId, "ORGN01", _dateTimeProvider, cancellationToken);
        }
    }
}
