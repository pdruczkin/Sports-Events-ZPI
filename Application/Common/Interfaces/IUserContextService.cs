using System.Security.Claims;

namespace Application.Common.Interfaces;

public interface IUserContextService
{
    ClaimsPrincipal User { get; }
    Guid? GetUserId { get; }
    bool HasAdminRole();
}
