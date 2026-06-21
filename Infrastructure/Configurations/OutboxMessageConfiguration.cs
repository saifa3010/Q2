using Domain.Outbox;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.Payload)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ProcessedAt)
                .IsRequired(false);

            builder.Property(x => x.Error)
                .IsRequired(false)
                .HasMaxLength(2000);

            // Partial index: only unprocessed messages need to be queried by the processor.
            builder.HasIndex(x => x.ProcessedAt)
                .HasFilter("[ProcessedAt] IS NULL")
                .HasDatabaseName("IX_OutboxMessages_Unprocessed");
        }
    }
}
