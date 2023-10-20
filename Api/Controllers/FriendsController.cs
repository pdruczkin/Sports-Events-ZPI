using Application.Friends.Commands.AcceptFriendInvitation;
using Application.Friends.Commands.BlockUser;
using Application.Friends.Commands.SendFriendInvitation;
using Application.Friends.Queries.GetFriendInvitations;
using Application.Friends.Queries.GetFriendsList;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class FriendsController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<FriendInvitationsDto>>> GetFriendsList()
        {
            var friendsUsernamesDtos = await Mediator.Send(new GetFriendsListQuery());
            return Ok(friendsUsernamesDtos);
        }

        [HttpGet("invitations")]
        public async Task<ActionResult<List<FriendInvitationsDto>>> GetFriendInvitations()
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

        [HttpPost("block")]
        public async Task<ActionResult> BlockUser([FromBody] BlockUserCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
