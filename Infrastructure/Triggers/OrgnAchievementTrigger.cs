using Application.Common.Interfaces;
using Application.Common.Models;
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

            if (organizedMeetingsCount <= 50)
            {
                if(organizedMeetingsCount == 50)
                {
                    await _dbContext.UserAchievements.AddAsync(new UserAchievement()
                    {
                        UserId = organizerId,
                        AchievementId = "ORGA50",
                        Obtained = _dateTimeProvider.Now
                    });
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                else if(organizedMeetingsCount == 10)
                {
                    await _dbContext.UserAchievements.AddAsync(new UserAchievement()
                    {
                        UserId = organizerId,
                        AchievementId = "ORGA10",
                        Obtained = _dateTimeProvider.Now
                    });
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                else if(organizedMeetingsCount == 5)
                {
                    await _dbContext.UserAchievements.AddAsync(new UserAchievement()
                    {
                        UserId = organizerId,
                        AchievementId = "ORGA05",
                        Obtained = _dateTimeProvider.Now
                    });
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                else if(organizedMeetingsCount == 1)
                {
                    await _dbContext.UserAchievements.AddAsync(new UserAchievement()
                    {
                        UserId = organizerId,
                        AchievementId = "ORGA01",
                        Obtained = _dateTimeProvider.Now
                    });
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            /*
            await _emailSendService.SendEmailAsync(new EmailDto() {
                To = "260369@student.pwr.edu.pl",
                Body = "TEST EMAIL - tekst",
                Subject = "TEST EMAIL"
            }); // Initial email*/
        }
        /*
        else if (context.ChangeType == ChangeType.Modified && context.Entity.Message != context.UnmodifiedEntity.Message)
        {
            _emailSendService.Send(email); // In case the content was updated we want to resent this email
        }*/

        return;// Task.FromResult();
    }
}