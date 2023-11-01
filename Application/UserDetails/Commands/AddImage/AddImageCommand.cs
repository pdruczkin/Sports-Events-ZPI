using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.UserDetails.Commands.AddImage;

public class AddImageCommand : IRequest<ImageDto>
{
    public IFormFile File { get; set; } = null;
}

public class AddImageCommandHandler : IRequestHandler<AddImageCommand, ImageDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IImageService _imageService;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;

    public AddImageCommandHandler(IApplicationDbContext dbContext, IImageService imageService, IUserContextService userContextService, IMapper mapper)
    {
        _dbContext = dbContext;
        _imageService = imageService;
        _userContextService = userContextService;
        _mapper = mapper;
    }
    
    public async Task<ImageDto> Handle(AddImageCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .Include(x => x.Image)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user is null) throw new AppException("User is not found");
        
        if (user.Image != null)
        {
            var deletionResult = await _imageService.DeleteImageAsync(user.Image.PublicId);
            if (deletionResult.Error != null) throw new AppException("Error occured in image deletion");
        }
        
        var result = await _imageService.AddImageAsync(request.File);
        
        if (result.Error != null) throw new AppException(result.Error.Message);
        
        var image = new Image
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.AbsoluteUri
        };

        user.Image = image;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var imageDto = _mapper.Map<ImageDto>(image);
        
        return imageDto;
    }
}