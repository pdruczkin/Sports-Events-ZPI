using Application.Common.Exceptions;
using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Achievements;

public class ChatAchievement
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ChatAchievement(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task CheckChatAchievementAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user == null) throw new AppException("User not found");

        await _dbContext.AddAchievementAsync(userId, "CHAT", _dateTimeProvider, cancellationToken);        
    }
}
