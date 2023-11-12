using Application.Common.Exceptions;
using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Achievements;

public class PartAchievement
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PartAchievement(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task CheckPartAchievement(Guid meetingId, CancellationToken cancellationToken)
    {
        var meeting = await _dbContext
            .Meetings
            .Include(x => x.MeetingParticipants)
            .FirstOrDefaultAsync(x => x.Id == meetingId);
        if (meeting == null) throw new AppException("Meeting not found");

        foreach (var mp in meeting.MeetingParticipants)
        {
            await GrantPartAchievement(mp.ParticipantId, cancellationToken);
        }
    }

    private async Task GrantPartAchievement(Guid userId, CancellationToken cancellationToken)
    {
        var participatedMeetingsCount = await _dbContext
            .MeetingParticipants
            .Include(x => x.Meeting)
            .CountAsync(x => x.ParticipantId == userId && x.Meeting!.EndDateTimeUtc < _dateTimeProvider.UtcNow && x.InvitationStatus == InvitationStatus.Accepted, cancellationToken);
        
        if (participatedMeetingsCount == 50)
        {
            await _dbContext.AddAchievementAsync(userId, "PART50", _dateTimeProvider, cancellationToken);
        }
        else if (participatedMeetingsCount == 10)
        {
            await _dbContext.AddAchievementAsync(userId, "PART10", _dateTimeProvider, cancellationToken);
        }
        else if (participatedMeetingsCount == 5)
        {
            await _dbContext.AddAchievementAsync(userId, "PART05", _dateTimeProvider, cancellationToken);
        }
        else if (participatedMeetingsCount == 1)
        {
            await _dbContext.AddAchievementAsync(userId, "PART01", _dateTimeProvider, cancellationToken);
        }
    }
}
