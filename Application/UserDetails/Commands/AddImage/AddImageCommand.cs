using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.UserDetails.Commands.AddImage;

public class AddImageCommand : IRequest<Unit>
{
    public IFormFile File { get; set; } = null;
}

public class AddImageCommandHandler : IRequestHandler<AddImageCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IImageService _imageService;
    private readonly IUserContextService _userContextService;

    public AddImageCommandHandler(IApplicationDbContext dbContext, IImageService imageService, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _imageService = imageService;
        _userContextService = userContextService;
    }
    
    public async Task<Unit> Handle(AddImageCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .Include(x => x.Image)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user is null) throw new AppException("User is not found");

        var result = await _imageService.AddImageAsync(request.File);
        
        if (result.Error != null) throw new AppException(result.Error.Message);
        
        if (user.Image != null)
        {
            var deletionResult = await _imageService.DeleteImageAsync(user.Image.PublicId);
            if (deletionResult.Error != null) throw new AppException("Error occured in image deletion");
        }
        
        var image = new Image
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.AbsolutePath
        };

        user.Image = image;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await Task.FromResult(Unit.Value);
    }
}