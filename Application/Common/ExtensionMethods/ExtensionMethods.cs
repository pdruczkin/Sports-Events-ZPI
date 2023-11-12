using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.ExtensionMethods;

public static class ExtensionMethods
{
    public static int CalculateAge(this DateTime dateOfBirth)
    {
        var age = DateTime.Now.Year - dateOfBirth.Year;
        if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
            age--;

        return age;
    }

    public static int CountMeetingParticipantsQuantity(this IApplicationDbContext dbContext, Guid meetingId)
    {
        var totalParticipantsQuantity = 1 + dbContext // 1 - meeting's organizer is also a participant
            .MeetingParticipants
            .Count(mp => mp.MeetingId == meetingId && mp.InvitationStatus == InvitationStatus.Accepted);
        return totalParticipantsQuantity;
    }

    public static async Task AddAchievementAsync(this IApplicationDbContext dbContext, Guid userId, string achievementId, IDateTimeProvider dateTimeProvider, CancellationToken cancellationToken)
    {
        if(!(await dbContext.HasAchievement(userId, achievementId, cancellationToken)))
        {
            await dbContext.UserAchievements.AddAsync(new UserAchievement()
            {
                UserId = userId,
                AchievementId = achievementId,
                Obtained = dateTimeProvider.Now
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public static async Task<bool> HasAchievement(this IApplicationDbContext dbContext, Guid userId, string achievementId, CancellationToken cancellationToken)
    {
        var userAchievement = await dbContext
            .UserAchievements
            .FirstOrDefaultAsync(x => x.UserId == userId && x.AchievementId == achievementId, cancellationToken);

        return userAchievement != null;
    }
}
