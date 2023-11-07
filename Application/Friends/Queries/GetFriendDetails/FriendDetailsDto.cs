using Application.Common.ExtensionMethods;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.UserDetails.Commands.ChangeUserDetails;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Friends.Queries.GetFriendDetails;

public class FriendDetailsDto : IMappable<User>
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? Age { get; set; }
    public Gender? Gender { get; set; }
    public ImageDto? Image { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<User, FriendDetailsDto>()
            .ForMember(u => u.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()));
    }
}

