using Application.Common.Mappings;
using Application.Common.Models;
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

    public UserIdentityDto Author { get; set; }


    public void Mapping(Profile profile)
    {
        profile.CreateMap<Post, PostDto>()
            .ForMember(x => x.Author, o => o.MapFrom(s => s.User));
    }
}