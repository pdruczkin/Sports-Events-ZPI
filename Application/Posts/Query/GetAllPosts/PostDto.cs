using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.Posts.Query.GetAllPosts;

public class PostDto : IMappable<Post>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Text { get; set; }
    
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public string Username { get; set; } = "";
    public string UserImageUrl { get; set; } = "";


    public void Mapping(Profile profile)
    {
        profile.CreateMap<Post, PostDto>()
            .ForMember(x => x.Username, o => o.MapFrom(s => s.User.Username))
            .ForMember(x => x.UserImageUrl, o => o.MapFrom(s => s.User.Image != null ? s.User.Image.Url : ""));
    }
}