using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Meetings.Queries.MeetingDetails.GetMeetingDetailsById;

public class MeetingDetailsDto : IMappable<Meeting>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public double Latitude { get; set; } // range: from -90 (South) to 90 (North)
    public double Longitude { get; set; } // range: from -180 (West) to 180 (East)
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime EndDateTimeUtc { get; set; }
    public MeetingVisibility Visibility { get; set; }
    public SportsDiscipline SportsDiscipline { get; set; }
    public Difficulty Difficulty { get; set; }
    public int MaxParticipantsQuantity { get; set; }
    public int CurrentParticipantsQuantity { get; set; }
    public int MinParticipantsAge { get; set; }
    public bool IsOrganizer { get; set; }

    public UserIdentityDto Organizer { get; set; }
    public List<UserIdentityDto> MeetingParticipants { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Meeting, MeetingDetailsDto>()
            .ForMember(x => x.MeetingParticipants, opt => opt.Ignore());
    }
}
