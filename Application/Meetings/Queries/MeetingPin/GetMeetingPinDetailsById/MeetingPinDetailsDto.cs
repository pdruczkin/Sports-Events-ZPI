using Application.Common.Mappings;
using Domain.Entities;
using Domain.Enums;

namespace Application.Meetings.Queries.MeetingPin.GetMeetingPinDetailsById;

public class MeetingPinDetailsDto : IMappable<Meeting>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime EndDateTimeUtc { get; set; }
    public SportsDiscipline SportsDiscipline { get; set; }
}