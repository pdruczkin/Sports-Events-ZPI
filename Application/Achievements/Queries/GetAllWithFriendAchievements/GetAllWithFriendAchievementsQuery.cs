using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;

namespace Application.Achievements.Queries.GetAllWithFriendAchievements;

public class GetAllWithFriendAchievementsQuery : IRequest<List<GroupedAchievementsDto>>
{
    public Guid FriendId { get; set; }
}

public class GetAllWithFriendAchievementsQueryHandler : IRequestHandler<GetAllWithFriendAchievementsQuery, List<GroupedAchievementsDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    public GetAllWithFriendAchievementsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<List<GroupedAchievementsDto>> Handle(GetAllWithFriendAchievementsQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.GetAllWithUserAchievements(request.FriendId, _mapper, cancellationToken);
    }
}

