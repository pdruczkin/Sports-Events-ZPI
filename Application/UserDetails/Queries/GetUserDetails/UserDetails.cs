using Application.Common.ExtensionMethods;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Friends.Queries.GetFriendDetails;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.UserDetails.Queries.GetUserDetails;

public class UserDetails : IMappable<User>
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public ImageDto? Image { get; set; }

    public bool HasAdminRole { get; set; }
    public IEnumerable<MeetingPinDto> RecentMeetings { get; set; } = new List<MeetingPinDto>();
    
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<User, UserDetails>()
            .ForMember(u => u.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()));
    }
}