namespace Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    public string Value { get; set; } = "";
    public DateTime SentAtUtc { get; set; }
    
    public Guid MeetingId { get; set; }
    public Meeting Meeting { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}