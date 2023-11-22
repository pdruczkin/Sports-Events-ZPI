using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;

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

        return await _dbContext.GetAllWithUserAchievements(userId, _mapper, cancellationToken);
    }
}