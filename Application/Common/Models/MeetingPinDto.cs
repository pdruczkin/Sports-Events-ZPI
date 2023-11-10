using Application.Common.ExtensionMethods;
using Application.Common.Mappings;
using Application.Friends.Queries.GetFriendDetails;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Models;

public class MeetingPinDto : IMappable<Meeting>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime EndDateTimeUtc { get; set; }
    public SportsDiscipline SportsDiscipline { get; set; }
    public string OrganizerUsername { get; set; } = "";
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Meeting, MeetingPinDto>()
            .ForMember(x => x.OrganizerUsername, o => o.MapFrom(s => s.Organizer.Username));
    }
}