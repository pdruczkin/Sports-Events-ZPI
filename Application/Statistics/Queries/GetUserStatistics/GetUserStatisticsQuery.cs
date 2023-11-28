using Application.Common.ExtensionMethods;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;

namespace Application.Statistics.Queries.GetUserStatistics;

public class GetUserStatisticsQuery : IRequest<StatisticsDto>
{

}

public class GetUserStatisticsQueryHandler : IRequestHandler<GetUserStatisticsQuery, StatisticsDto>
{ 
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public GetUserStatisticsQueryHandler(IApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider, IMapper mapper, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
        _userContextService = userContextService;
    }

    public async Task<StatisticsDto> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;

        var statisticsDto = await _dbContext.GetUserStatistics(userId, _dateTimeProvider,  _mapper, cancellationToken);        
        
        return statisticsDto;
    }
}








