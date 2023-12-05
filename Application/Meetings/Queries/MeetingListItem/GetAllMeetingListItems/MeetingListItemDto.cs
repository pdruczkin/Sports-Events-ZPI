using Application.Common.Mappings;
using Domain.Entities;
using Domain.Enums;

namespace Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;

public class MeetingListItemDto : IMappable<Meeting>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime Created { get; set; }
    public double Latitude { get; set; } // range: from -90 (South) to 90 (North)
    public double Longitude { get; set; } // range: from -180 (West) to 180 (East)
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime EndDateTimeUtc { get; set; }
    public SportsDiscipline SportsDiscipline { get; set; }
    public Difficulty Difficulty { get; set; }
    public int MaxParticipantsQuantity { get; set; }
    public int CurrentParticipantsQuantity { get; set; }
    public int MinParticipantsAge { get; set; }
    public MeetingVisibility Visibility { get; set; }
    public string OrganizerUsername { get; set; }

}
