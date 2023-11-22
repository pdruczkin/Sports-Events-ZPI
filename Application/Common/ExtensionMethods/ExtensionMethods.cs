using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
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
                Obtained = dateTimeProvider.UtcNow
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

    public static async Task<List<GroupedAchievementsDto>> GetAllWithUserAchievements(this IApplicationDbContext dbContext, Guid? userId, IMapper mapper, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var userAchievements = await dbContext
            .UserAchievements
            .Where(ua => ua.UserId == userId)
            .ToListAsync(cancellationToken);

        var achievements = await dbContext
            .Achievements
            .GroupBy(x => x.Category)
            .Select(x => new GroupedAchievementsDto()
            {
                Category = x.Key.ToString(),
                Achievements = mapper.Map<List<AchievementDetailsDto>>(x.ToList())
            })
            .ToListAsync(cancellationToken);

        achievements.ForEach(x => x.Achievements
            .ToList()
            .ForEach(a => {
                var obtainedDateTimeUtc = userAchievements
                    .Where(x => x.AchievementId == a.Id)
                    .Select(x => x.Obtained)
                    .FirstOrDefault();
                a.Obtained = obtainedDateTimeUtc.Equals(default) ? null : obtainedDateTimeUtc;
            }));

        return achievements;
    }

    public static async Task<List<GroupedAchievementsDto>> GetUserAchievements(this IApplicationDbContext dbContext, Guid? userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var userAchievements = await dbContext
            .UserAchievements
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Achievement)
            .ToListAsync(cancellationToken);

        var groupedUserAchievements = userAchievements
            .Select(ua => new
            {
                Category = ua.Achievement.Category.ToString(),
                AchievementDetailsDto = new AchievementDetailsDto()
                {
                    Id = ua.AchievementId,
                    Description = ua.Achievement.Description,
                    Obtained = ua.Obtained
                }
            })
            .GroupBy(
                x => x.Category,
                x => x.AchievementDetailsDto,
                (category, achievements) => new GroupedAchievementsDto()
                {
                    Category = category,
                    Achievements = achievements.ToList()
                }
            )
            .ToList();

        return groupedUserAchievements;
    }
}
