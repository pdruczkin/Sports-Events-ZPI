using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Achievements.Queries.GetAllAchievements;

public class GroupedAchievementsDto : IMappable<Achievement>
{
    public string Category { get; set; }
    public IEnumerable<AchievementDetailsDto> Achievements { get; set; }
}
