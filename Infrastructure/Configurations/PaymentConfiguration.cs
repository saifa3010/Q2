using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.InvoiceId)
                   .IsRequired();

            builder.Property(x => x.PaymentDate)
                   .IsRequired();

            builder.Property(x => x.ReferenceNumber)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.Notes)
                   .HasMaxLength(500)
                   .IsRequired(false);


            builder.OwnsOne(x => x.Amount, money =>
            {
                money.Property(x => x.Amount)
                     .HasColumnName("Amount")
                     .IsRequired();
            });
        }
    }
}