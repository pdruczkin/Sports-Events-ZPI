using Application.UserDetails.Commands.AddImage;
using Application.UserDetails.Commands.ChangePassword;
using Application.UserDetails.Commands.ChangeUserDetails;
using Application.UserDetails.Commands.DeleteImage;
using Application.UserDetails.Queries.GetUserDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Digests;

namespace Api.Controllers;

public class UserDetailsController : ApiControllerBase
{
    [HttpPut("change-password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await Mediator.Send(command);

        return Ok();
    }
    
    [HttpGet]
    public async Task<ActionResult> GetUserDetails()
    {
        var detailsDto = await Mediator.Send(new GetUserDetailsCommand());

        return Ok(detailsDto);
    }

    [HttpPut]
    public async Task<ActionResult> ChangeUserDetails([FromBody] ChangeUserDetailsCommand command)
    {
        await Mediator.Send(command);
        
        return Ok();
    }

    [HttpPost("image")]
    public async Task<ActionResult> AddImage(IFormFile file)
    {
        var command = new AddImageCommand{File = file};
        var imageDto = await Mediator.Send(command);

        return Ok(imageDto);
    }

    [HttpDelete("image")]
    public async Task<ActionResult> DeleteImage([FromBody] string publicId)
    {
        var command = new DeleteImageCommand{ PublicId = publicId };
        await Mediator.Send(command);

        return NoContent();
    }
}