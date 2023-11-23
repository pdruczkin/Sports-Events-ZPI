using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.HasKey(x => new { x.UserId, x.AchievementId });

        builder
            .HasOne(ua => ua.Achievement)
            .WithMany(a => a.UserAchievements)
            .HasForeignKey(a => a.AchievementId);

        builder
            .HasOne(ua => ua.User)
            .WithMany(u => u.UserAchievements)
            .HasForeignKey(ua => ua.UserId);

        builder.Property(x => x.AchievementId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Obtained).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
