using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Achievements.Queries.GetUserAchievements;

public class GetUserAchievementsQuery : IRequest<List<GroupedAchievementsDto>>
{

}

public class GetUserAchievementsQueryHandler : IRequestHandler<GetUserAchievementsQuery, List<GroupedAchievementsDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;
    public GetUserAchievementsQueryHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IMapper mapper)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _mapper = mapper;
    }
    public async Task<List<GroupedAchievementsDto>> Handle(GetUserAchievementsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var userAchievements = await _dbContext
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
