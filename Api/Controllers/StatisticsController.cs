using Application.Achievements.Queries.GetAllWithFriendAchievements;
using Application.Common.Models;
using Application.Statistics.Queries.GetFriendStatistics;
using Application.Statistics.Queries.GetUserStatistics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class StatisticsController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<StatisticsDto>> GetUserStatistics()
        {
            var statisticsDto = await Mediator.Send(new GetUserStatisticsQuery());
            return Ok(statisticsDto);
        }

        [HttpGet("{friendId}")]
        public async Task<ActionResult<StatisticsDto>> GetFriendStatistics(Guid friendId)
        {
            var statisticsDto = await Mediator.Send(new GetFriendStatisticsQuery() { FriendId = friendId });
            return Ok(statisticsDto);
        }
    }
}
