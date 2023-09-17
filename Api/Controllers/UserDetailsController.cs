using Application.UserDetails.Commands.ChangePassword;
using Application.UserDetails.Queries.GetUserDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class UserDetailsController : ApiControllerBase
{
    [HttpPut("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await Mediator.Send(command);

        return Ok();
    }


    [HttpGet]
    [Authorize]
    public async Task<ActionResult> GetUserDetails()
    {
        var detailsDto = await Mediator.Send(new GetUserDetailsCommand());

        return Ok(detailsDto);
    }
    
}