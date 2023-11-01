using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UserDetails.Commands.DeleteImage;

public class DeleteImageCommand : IRequest<Unit>
{
    public string PublicId { get; set; } = "";
}

public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand, Unit>
{
    
    private readonly IApplicationDbContext _dbContext;
    private readonly IImageService _imageService;
    private readonly IUserContextService _userContextService;

    public DeleteImageCommandHandler(IApplicationDbContext dbContext, IImageService imageService, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _imageService = imageService;
        _userContextService = userContextService;
    }

    
    public async Task<Unit> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .Include(x => x.Image)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user is null) throw new AppException("User is not found");

        if (user.Image == null) throw new AppException("User doesn't have uploaded image");
        if (user.Image.PublicId != request.PublicId) throw new AppException("Provided image publicId doesn't match user image publicId");
        
        var deletionResult = await _imageService.DeleteImageAsync(user.Image.PublicId);
        if (deletionResult.Error != null) throw new AppException("Error occured in image deletion");

        user.Image = null;
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        
        return await Task.FromResult(Unit.Value);
    }
}