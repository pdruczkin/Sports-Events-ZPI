using Application.Account.Commands.DeleteAccount;
using Application.Account.Commands.ForgotPassword;
using Application.Account.Commands.LoginUser;
using Application.Account.Commands.RegisterUser;
using Application.Account.Commands.ResetPassword;
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
    public async Task<ActionResult> Verify([FromBody] string token)
    {
        await Mediator.Send(new VerifyAccountCommand { Token = token });

        return NoContent();
    }
    
    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] string email)
    {
        await Mediator.Send(new ForgotPasswordCommand { Email = email });

        return NoContent();
    }
    
    [HttpPut("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await Mediator.Send(command);

        return NoContent();
    }
}