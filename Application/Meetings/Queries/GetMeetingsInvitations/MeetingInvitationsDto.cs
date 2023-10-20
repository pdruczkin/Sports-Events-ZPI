using Application.Account.Commands.RegisterUser;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Meetings.Queries.GetMeetingsInvitations;

public class MeetingInvitationsDto : IMappable<Meeting>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime EndDateTimeUtc { get; set; }
    public SportsDiscipline SportsDiscipline { get; set; }
    public Difficulty Difficulty { get; set; }

    public string OrganizerUsername { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Meeting, MeetingInvitationsDto>()
            .ForMember(dto => dto.OrganizerUsername, o => o.MapFrom(m => m.Organizer.Username));
    }
}