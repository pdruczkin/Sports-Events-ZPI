using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasKey(x => x.PublicId);

        builder.HasOne(x => x.User)
            .WithOne(u => u.Image)
            .HasForeignKey<Image>(x => x.UserId);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(256);
    }
}