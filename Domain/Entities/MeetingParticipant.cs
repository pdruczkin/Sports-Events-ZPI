using Domain.Enums;

namespace Domain.Entities;

public class MeetingParticipant
{
    public Guid MeetingId { get; set; }
    public Guid UserId { get; set; }
    public InvitationStatus InvitationStatus { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public User? Participant { get; set; }
    public Meeting? Meeting { get; set; }
}

