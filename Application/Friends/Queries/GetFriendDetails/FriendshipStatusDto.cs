using Domain.Enums;

namespace Application.Friends.Queries.GetFriendDetails;

public class FriendshipStatusDto 
{
    public FriendshipStatus? Status { get; set; }
    public bool? IsOriginated { get; set; }
}