using Application.Achievements.Queries.GetAllAchievements;
using Application.Achievements.Queries.GetUserAchievements;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class AchievementsController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpGet("all")]
    public async Task<ActionResult<List<GroupedAchievementsDto>>> GetAll()
    {
        var groupedAchievementsDtos = await Mediator.Send(new GetAllAchievementsQuery());
        return Ok(groupedAchievementsDtos);
    }

    [HttpGet]
    public async Task<ActionResult<List<GroupedAchievementsDto>>> GetUserAchievements()
    {
        var groupedUserAchievements = await Mediator.Send(new GetUserAchievementsQuery());
        return Ok(groupedUserAchievements); 
    }
}
