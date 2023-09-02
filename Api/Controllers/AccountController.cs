using Application.Account.Commands.LoginUser;
using Application.Account.Commands.RegisterUser;
using Application.Account.Commands.VerifyAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[AllowAnonymous]
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

    [HttpPost("verify")]
    public async Task<ActionResult> Verify([FromQuery] string token)
    {
        await Mediator.Send(new VerifyAccountCommand() { Token = token });

        return NoContent();
    }
}