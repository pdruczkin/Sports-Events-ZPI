using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Achievements.Queries.GetUserAchievements;

public class GetUserAchievementsQuery : IRequest<List<GroupedAchievementsDto>>
{

}

public class GetUserAchievementsQueryHandler : IRequestHandler<GetUserAchievementsQuery, List<GroupedAchievementsDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    public GetUserAchievementsQueryHandler(IApplicationDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    public async Task<List<GroupedAchievementsDto>> Handle(GetUserAchievementsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;

        return await _dbContext.GetUserAchievements(userId, cancellationToken);
    }
}
