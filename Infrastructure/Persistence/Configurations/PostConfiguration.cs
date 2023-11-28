using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Posts)
            .HasForeignKey(x => x.UserId);

        builder.Property(x => x.Title)
            .HasMaxLength(100);
        
        builder.Property(x => x.Description)
            .HasMaxLength(400);
        
        builder.Property(x => x.Text)
            .HasMaxLength(4000);
    }
}