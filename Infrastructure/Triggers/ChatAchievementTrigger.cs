using Application.Achievements;
using Application.Common.Interfaces;
using Domain.Entities;
using EntityFrameworkCore.Triggered;
using System;

namespace Infrastructure.Triggers;

public class ChatAchievementTrigger : IAfterSaveTrigger<ChatMessage>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ChatAchievementTrigger(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task AfterSave(ITriggerContext<ChatMessage> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Added)
        {
            var chatAchievementChecker = new ChatAchievement(_dbContext, _dateTimeProvider);
            await chatAchievementChecker.CheckChatAchievementAsync(context.Entity.UserId, cancellationToken);
        }
    }
}