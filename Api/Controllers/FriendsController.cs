using Application.Friends.Commands.AcceptFriendInvitation;
using Application.Friends.Commands.SendFriendInvitation;
using Application.Friends.Queries.GetFriendInvitations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class FriendsController : ApiControllerBase
    {
        [HttpGet("invitations")]
        public async Task<ActionResult> GetFriendInvitations()
        {
            var friendInvitations = await Mediator.Send(new GetFriendInvitationsQuery());
            return Ok(friendInvitations);
        }

        [HttpPost("invite")]
        public async Task<ActionResult> SendFriendInvitation([FromBody] SendFriendInvitationCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost("accept")]
        public async Task<ActionResult> AcceptFriendInvitation([FromBody] AcceptFriendInvitationCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
