using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.InviterId).IsRequired();
        builder.Property(f => f.InviteeId).IsRequired();
        builder.Property(f => f.FriendshipStatus).IsRequired();
        builder.Property(f => f.StatusDateTimeUtc).IsRequired();

        builder.HasOne(f => f.Inviter)
            .WithMany(u => u.AsInviter)
            .HasForeignKey(f => f.InviterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Invitee)
            .WithMany(u => u.AsInvitee)
            .HasForeignKey(f => f.InviteeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
