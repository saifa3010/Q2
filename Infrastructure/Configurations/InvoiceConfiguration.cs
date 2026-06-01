using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StatusId)
                   .IsRequired();

            // Ignore domain events
            //builder.Ignore(x => x.DomainEvents);

            builder.OwnsOne(x => x.TotalAmount, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("TotalAmount")
                     .IsRequired();
            });

            builder.OwnsOne(x => x.PaidAmount, money =>
            {
                money.Property(m => m.Amount)
                     .HasColumnName("PaidAmount")
                     .IsRequired();
            });

            builder.OwnsMany(x => x.Items, item =>
            {
                item.WithOwner().HasForeignKey("InvoiceId");

                item.Property<int>("Id"); // shadow key
                item.HasKey("Id");

                item.Property(i => i.Name)
                    .IsRequired();

                item.Property(i => i.Quantity)
                    .IsRequired();

                item.OwnsOne(i => i.Price, price =>
                {
                    price.Property(p => p.Amount)
                         .HasColumnName("Price")
                         .IsRequired();
                });
            });

            builder.Metadata.FindNavigation(nameof(Invoice.Items))
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        }

    }
}
