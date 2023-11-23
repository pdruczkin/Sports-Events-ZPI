using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Common.Models;

public class GroupedAchievementsDto : IMappable<Achievement>
{
    public string Category { get; set; }
    public IEnumerable<AchievementDetailsDto> Achievements { get; set; }
}
