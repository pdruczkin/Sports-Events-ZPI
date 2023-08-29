using Application.Common.Mappings;
using Domain.Entities;
using Domain.Enums;

namespace Application.Meetings.Queries.MeetingPin.GetAllMeetingPins;

public class MeetingPinDto : IMappable<Meeting>
{
    public Guid Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public SportsDiscipline SportsDiscipline { get; set; }
}
