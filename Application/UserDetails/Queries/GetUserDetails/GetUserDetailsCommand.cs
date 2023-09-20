using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UserDetails.Queries.GetUserDetails;

public class GetUserDetailsCommand : IRequest<UserDetails>
{
    
}

public class GetUserDetailsCommandHandler : IRequestHandler<GetUserDetailsCommand, UserDetails>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public GetUserDetailsCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
    }
    
    public async Task<UserDetails> Handle(GetUserDetailsCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user is null) throw new AppException("User is not found");

        var userDetails = _mapper.Map<UserDetails>(user);

        return userDetails;
    }
}



