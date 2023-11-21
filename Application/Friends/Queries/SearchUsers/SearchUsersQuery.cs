using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Friends.Queries.SearchUsers;

public class SearchUsersQuery : IRequest<List<UserIdentityDto>>
{
    public string SearchPhrase { get; set; }
}

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, List<UserIdentityDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public SearchUsersQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, IUserContextService userContextService)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
        _userContextService = userContextService;
    }
    public async Task<List<UserIdentityDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.GetUserId;

        var users = await _applicationDbContext
            .Users
            .Include(x => x.Image)
            .Where(x => x.Username.ToLower().Contains(request.SearchPhrase.ToLower()))
            .Where(x => x.Id != userId)
            .Take(20)
            .ToListAsync();

        var userIdentities = _mapper.Map<List<UserIdentityDto>>(users);

        return userIdentities;
    }
}
