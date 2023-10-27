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

    public SearchUsersQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }
    public async Task<List<UserIdentityDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _applicationDbContext
            .Users
            .Where(x => x.Username.Contains(request.SearchPhrase))
            .Take(20)
            .ToListAsync();

        var userIdentities = _mapper.Map<List<UserIdentityDto>>(users);

        return userIdentities;
    }
}
