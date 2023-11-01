using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

    Guid? IUserContextService.GetUserId => VerifyUserClaims();

    public Guid? VerifyUserClaims()
    {
        if (User is null) return null;

        List<Claim> claims = User.Claims.Where(c => c.Type == ClaimTypes.Role.ToString()).ToList();

        if ((claims == null) || (claims?.Count == 0)) return null;

        return Guid.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
    }
}
