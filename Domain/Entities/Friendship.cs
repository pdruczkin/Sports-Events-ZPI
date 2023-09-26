using Domain.Enums;

namespace Domain.Entities;

public class Friendship
{
    public Guid Id { get; set; }
    public Guid InviterId { get; set; }
    public User Inviter { get; set; }
    public Guid InviteeId { get; set;}
    public User Invitee { get; set; }
    public FriendshipStatus FriendshipStatus { get; set; }
    public DateTime StatusDateTimeUtc { get; set; }
}
