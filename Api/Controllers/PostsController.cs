using Application.Common.Models;
using Application.Posts.Commands.AddPost;
using Application.Posts.Commands.DeletePost;
using Application.Posts.Commands.UpdatePost;
using Application.Posts.Query.GetAllPosts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class PostsController : ApiControllerBase
{
    [AllowAnonymous] 
    [HttpGet("all")] 
    public async Task<ActionResult<PagedResult<PostDto>>> GetAllPosts([FromQuery] GetAllPostsQuery query)
    {
        var posts = await Mediator.Send(query);
        return Ok(posts);
    }
    
    [HttpPost]
    public async Task<ActionResult> AddPost([FromBody] AddPostCommand command)
    {
        await Mediator.Send(command);
        return Created("", null);
    }

    [HttpPut]
    public async Task<ActionResult> UpdatePost([FromBody] UpdatePostCommand command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    [HttpDelete("{postId}")]
    public async Task<ActionResult> DeletePost([FromRoute] Guid postId)
    {
        await Mediator.Send(new DeletePostCommand{PostId = postId});
        return NoContent();
    }

}