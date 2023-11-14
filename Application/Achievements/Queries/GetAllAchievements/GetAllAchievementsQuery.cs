using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Achievements.Queries.GetAllAchievements;

public class GetAllAchievementsQuery : IRequest<List<GroupedAchievementsDto>>
{

}

public class GetAllAchievementsHandler : IRequestHandler<GetAllAchievementsQuery, List<GroupedAchievementsDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    public GetAllAchievementsHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<List<GroupedAchievementsDto>> Handle(GetAllAchievementsQuery request, CancellationToken cancellationToken)
    {
        var achievements = await _dbContext
            .Achievements
            .GroupBy (x => x.Category)
            .Select(x => new GroupedAchievementsDto()
            {
                Category = x.Key.ToString(),
                Achievements = _mapper.Map< List<AchievementDetailsDto>>(x.ToList())
            })
            .ToListAsync(cancellationToken);

        return achievements;
    }
}
