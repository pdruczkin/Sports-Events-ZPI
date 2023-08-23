using Application.Account.Commands;
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
    
}