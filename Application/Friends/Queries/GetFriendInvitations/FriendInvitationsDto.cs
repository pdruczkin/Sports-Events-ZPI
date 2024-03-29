﻿using Application.Common.Models;

namespace Application.Friends.Queries.GetFriendInvitations;

public class FriendInvitationsDto
{
    public Guid InviterId { get; set; }
    public string InviterUsername { get; set; }
    public DateTime InvitationDateTimeUtc { get; set; }
    public ImageDto? Image { get; set; }
}
