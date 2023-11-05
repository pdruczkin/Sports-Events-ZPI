using Domain.Enums;

namespace Domain.Entities;

public class Achievement
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public AchievementCategory Category { get; set; }
    //public Image Image { get; set; }

    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}
