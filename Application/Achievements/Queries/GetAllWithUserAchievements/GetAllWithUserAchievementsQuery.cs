using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Achievements.Queries.GetAllWithUserAchievements;

public class GetAllWithUserAchievementsQuery : IRequest<List<GroupedAchievementsDto>>
{

}

public class GetAllWithUserAchievementsQueryHandler : IRequestHandler<GetAllWithUserAchievementsQuery, List<GroupedAchievementsDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    public GetAllWithUserAchievementsQueryHandler(IApplicationDbContext dbContext, IMapper mapper, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
    }
    public async Task<List<GroupedAchievementsDto>> Handle(GetAllWithUserAchievementsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var userAchievements = await _dbContext
            .UserAchievements
            .Where(ua => ua.UserId == userId)
            .ToListAsync(cancellationToken);

        var achievements = await _dbContext
            .Achievements
            .GroupBy(x => x.Category)
            .Select(x => new GroupedAchievementsDto()
            {
                Category = x.Key.ToString(),
                Achievements = _mapper.Map<List<AchievementDetailsDto>>(x.ToList())
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
}