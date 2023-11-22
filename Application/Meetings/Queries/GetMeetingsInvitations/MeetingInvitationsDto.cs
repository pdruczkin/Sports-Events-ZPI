using Application.Account.Commands.RegisterUser;
using Application.Common.Mappings;
using Application.Common.Models;
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

    public UserIdentityDto Organizer { get; set; }
}