﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Infrastructure.Contexts;

namespace OrderService.Infrastructure.EntityConfigurations
{
    public class BuyerEntityConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.ToTable("buyers", OrderDbContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.Id);

            builder.Ignore(b => b.DomainEvents);

            builder.Property(i => i.Id).ValueGeneratedOnAdd();

            builder.Property(i => i.Name).HasColumnType("name").HasColumnType("varchar").HasMaxLength(100);

            builder.HasMany(i => i.PaymentMethods)
                .WithOne()
                .HasForeignKey(i => i.Id)
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
