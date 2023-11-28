using Application.Common.Models;
using Application.Posts.Commands.AddPost;
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

}