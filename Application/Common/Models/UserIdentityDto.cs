using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Common.Models;

public class UserIdentityDto : IMappable<User>
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public ImageDto? Image { get; set; }
}
