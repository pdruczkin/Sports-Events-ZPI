using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UserDetails.Commands.ChangeUserDetails;

public class ChangeUserDetailsCommand : IRequest<Unit>, IMappable<User>
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<ChangeUserDetailsCommand, User>()
            .ForMember(u => u.DateOfBirth, o => o.MapFrom(s => new DateTime(s.DateOfBirth.Year, s.DateOfBirth.Month, s.DateOfBirth.Day)));
    }
}

public class ChangeUserDetailsCommandHandler : IRequestHandler<ChangeUserDetailsCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public ChangeUserDetailsCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
    }
    
    public async Task<Unit> Handle(ChangeUserDetailsCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user is null) throw new AppException("User is not found");

        if (_dbContext.Users.Any(x => x.Email == request.Email && x.Id != userId))
        {
            throw new AppException("Email must be unique");
        }
        
        if (_dbContext.Users.Any(x => x.Username == request.Username && x.Id != userId))
        {
            throw new AppException("Username must be unique");
        }
        
        _mapper.Map(request, user);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return await Task.FromResult(Unit.Value);
    }
}



