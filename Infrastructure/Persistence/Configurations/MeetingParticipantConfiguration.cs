using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MeetingParticipantConfiguration : IEntityTypeConfiguration<MeetingParticipant>
{
    public void Configure(EntityTypeBuilder<MeetingParticipant> builder)
    {
        builder.HasKey(x => new { x.UserId, x.MeetingId });

        builder
            .HasOne(x => x.Meeting)
            .WithMany(m => m.MeetingParticipants)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(x => x.Participant)
            .WithMany(u => u.MeetingParticipants)
            .OnDelete(DeleteBehavior.NoAction);
    }
}