using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Posts.Commands.DeletePost;

public class DeletePostCommand : IRequest<Unit>
{
    public Guid PostId { get; set; }
}

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public DeletePostCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    
    public async Task<Unit> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;

        var user = await _dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("Logged user is not found");
        
        var post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.Id == request.PostId, cancellationToken);
        if (post is null) throw new AppException($"Post of id {request.PostId} is not found");

        if (user.Role != Role.Administrator && post.UserId != userId) throw new AppException($"You're not allowed to update this post");

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return await Task.FromResult(Unit.Value);
    }
}