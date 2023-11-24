using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Posts.Commands.AddPost;

public class AddPostCommand : IRequest<Unit>, IMappable<Post>
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string Text { get; set; } = "";
}

public class AddPostCommandHandler : IRequestHandler<AddPostCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AddPostCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Unit> Handle(AddPostCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;

        var user = await _dbContext
            .Users
            .Include(x => x.Posts)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("Logged user is not found");

        var userLastPosts = user.Posts.OrderByDescending(x => x.Created).Take(3).ToList();
        if (userLastPosts.Count == 3 && userLastPosts[2].Created.AddHours(1) > _dateTimeProvider.UtcNow)
            throw new AppException("You're allowed to upload maximum of 3 in an hour");
        
        
        var post = _mapper.Map<Post>(request);
        post.User = user;

        _dbContext.Posts.Add(post);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }
}