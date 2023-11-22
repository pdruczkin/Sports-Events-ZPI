using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Friends.Queries.GetFriendsList;

public class FriendUsernameDto
{
    public Guid Id { get; set; }
    public string FriendUsername { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ImageDto? Image { get; set; }
}
