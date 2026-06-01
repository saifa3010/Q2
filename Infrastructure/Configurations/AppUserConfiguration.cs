using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.KeycloakUserId)
                   .IsRequired();

            builder.HasIndex(x => x.KeycloakUserId)
                   .IsUnique();

            builder.Property(x => x.FullName)
                   .HasMaxLength(200);

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.PhoneNumber)
                   .HasMaxLength(30);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();
        }
    }
}