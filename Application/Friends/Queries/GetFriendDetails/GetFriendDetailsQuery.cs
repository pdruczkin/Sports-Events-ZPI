using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Friends.Queries.GetFriendDetails;

public class GetFriendDetailsQuery : IRequest<FriendDetailsDto>
{
    public Guid Id { get; set; }
}

public class GetFriendDetailsQueryHandler : IRequestHandler<GetFriendDetailsQuery, FriendDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;

    public GetFriendDetailsQueryHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IMapper mapper)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
        _mapper = mapper;
    }
    
    public async Task<FriendDetailsDto> Handle(GetFriendDetailsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;
        var user = await _dbContext
            .Users
            .Include(x => x.AsInvitee)
            .Include(x => x.AsInviter)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user is null) throw new AppException("User is not found");

        var friend = await _dbContext
            .Users
            .Include(x => x.Image)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        var friendDetailsDto = _mapper.Map<FriendDetailsDto>(friend);
        
        var friendship = user.AsInvitee.Where(x => x.InviterId == request.Id)
            .Union(user.AsInviter.Where(x => x.InviteeId == request.Id))
            .OrderByDescending(x => x.StatusDateTimeUtc)
            .ToList();

        if (!friendship.Any() || friendship.First().FriendshipStatus != FriendshipStatus.Accepted)
        {
            friendDetailsDto.FirstName = null;
            friendDetailsDto.LastName = null;
            friendDetailsDto.Age = null;
            friendDetailsDto.Gender = null;
        }

        return friendDetailsDto;
    }
}