using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.Username).HasMaxLength(256);
        builder.Property(x => x.FirstName).HasMaxLength(256);
        builder.Property(x => x.LastName).HasMaxLength(256);
    }
}