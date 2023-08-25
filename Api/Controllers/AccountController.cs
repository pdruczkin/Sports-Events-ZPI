using Application.Account.Commands;
using Application.Account.Commands.LoginUser;
using Application.Account.Commands.RegisterUser;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class AccountController : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var id = await Mediator.Send(command);
        
        return Created(id, null);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginUserCommand command)
    {
        var jwtToken = await Mediator.Send(command);

        return Ok(jwtToken);
    }

}