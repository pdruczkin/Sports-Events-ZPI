using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Posts.Commands.UpdatePost;

public class UpdatePostCommand : IMappable<Post>, IRequest<Unit>
{
    public Guid  PostId { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string Text { get; set; } = "";
}

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;

    public UpdatePostCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IMapper mapper)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _mapper = mapper;
    }
    
    public async Task<Unit> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;

        var user = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("Logged user is not found");

        var post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.Id == request.PostId, cancellationToken);
        if (post is null) throw new AppException($"Post of id {request.PostId} is not found");

        if (user.Role != Role.Administrator && post.UserId != userId) throw new AppException($"You're not allowed to update this post");

        _mapper.Map(request, post);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }
}