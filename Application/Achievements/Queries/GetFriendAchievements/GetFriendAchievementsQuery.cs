using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Achievements.Queries.GetFriendAchievements;

public class GetFriendAchievementsQuery : IRequest<List<GroupedAchievementsDto>>
{
    public Guid FriendId { get; set; }
}

public class GetFriendAchievementsQueryHandler : IRequestHandler<GetFriendAchievementsQuery, List<GroupedAchievementsDto>>
{
    private readonly IApplicationDbContext _dbContext;
    public GetFriendAchievementsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<GroupedAchievementsDto>> Handle(GetFriendAchievementsQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.GetUserAchievements(request.FriendId, cancellationToken);
    }
}
