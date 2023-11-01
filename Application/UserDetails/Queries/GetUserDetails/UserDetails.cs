using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;

namespace Application.UserDetails.Queries.GetUserDetails;

public class UserDetails : IMappable<User>
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public ImageDto Image { get; set; }
}