using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User;
    public Guid? GetUserId => User is not null && User.Identity.IsAuthenticated ? Guid.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value) : null;
}
