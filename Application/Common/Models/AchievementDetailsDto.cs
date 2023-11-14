using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Common.Models;

public class AchievementDetailsDto : IMappable<Achievement>
{
    public string Id { get; set; }
    public string Description { get; set; }
    //public Image Image { get; set; }
    public DateTime? Obtained { get; set; } = null;
}
