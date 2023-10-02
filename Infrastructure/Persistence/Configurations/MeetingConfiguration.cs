using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MeetingConfiguration  : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title).IsRequired().HasMaxLength(256);
            builder.Property(m => m.Created).IsRequired();
            builder.Property(m => m.Description).HasMaxLength(8192);
            builder.Property(m => m.Latitude).IsRequired().HasColumnType("decimal(8,6)");
            builder.Property(m => m.Longitude).IsRequired().HasColumnType("decimal(9,6)");
            builder.Property(m => m.StartDateTimeUtc).IsRequired();
            builder.Property(m => m.EndDateTimeUtc).IsRequired();

            builder.HasOne(m => m.Organizer)
                .WithMany(u => u.OrganizedEvents);
        }
    }
}
