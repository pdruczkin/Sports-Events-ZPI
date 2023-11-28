using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Posts.Query.GetAllPosts;

public class GetAllPostsQuery : IRequest<PagedResult<PostDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, PagedResult<PostDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllPostsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public async Task<PagedResult<PostDto>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await _dbContext
            .Posts
            .Include(x => x.User).ThenInclude(x => x.Image)
            .OrderByDescending(x => x.Created)
            .ToListAsync(cancellationToken);
            
        var pagedPosts = GetPagedPosts(posts, request.PageSize, request.PageNumber);

        var count = posts.Count;

        var pagedPostsDto = _mapper.Map<List<PostDto>>(pagedPosts);

        var pagedResult = new PagedResult<PostDto>(pagedPostsDto, count, request.PageNumber, request.PageSize);

        return pagedResult;
    }
    
    private static List<Post> GetPagedPosts(IEnumerable<Post> posts, int pageSize, int pageNumber)
    {
        var pagedPosts = posts
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToList();

        return pagedPosts;
    }
}