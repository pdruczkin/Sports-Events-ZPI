using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Statistics.Queries;

public class FavoriteParticipantDto
{
    public UserIdentityDto? UserIdentityDto { get; set; }
    public int Count { get; set; }
}
