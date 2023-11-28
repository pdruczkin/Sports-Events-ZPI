using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;

namespace Application.Statistics.Queries.GetFriendStatistics;

public class GetFriendStatisticsQuery : IRequest<StatisticsDto>
{
    public Guid FriendId { get; set; }
}

public class GetFriendStatisticsQueryHandler : IRequestHandler<GetFriendStatisticsQuery, StatisticsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;
    
    public GetFriendStatisticsQueryHandler(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider, IMapper mapper)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }
    public async Task<StatisticsDto> Handle(GetFriendStatisticsQuery request, CancellationToken cancellationToken)
    {
        var statisticsDto = await _dbContext.GetUserStatistics(request.FriendId, _dateTimeProvider, _mapper, cancellationToken);

        return statisticsDto;
    }
}

