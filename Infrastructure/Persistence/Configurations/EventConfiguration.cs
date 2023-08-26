using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class EventConfiguration  : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title).IsRequired().HasMaxLength(256);
            builder.Property(e => e.Created).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(8192);
            builder.Property(e => e.Latitude).IsRequired().HasColumnType("decimal(8,6)");
            builder.Property(e => e.Longitude).IsRequired().HasColumnType("decimal(9,6)");
            builder.Property(e => e.StartDateTime).IsRequired();
            builder.Property(e => e.EndDateTime).IsRequired();

            builder.HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents);

          //  builder.HasMany(e => e.Participants)
          //     .WithMany(u => u.ParticipatedEvents);
        }
    }
}
