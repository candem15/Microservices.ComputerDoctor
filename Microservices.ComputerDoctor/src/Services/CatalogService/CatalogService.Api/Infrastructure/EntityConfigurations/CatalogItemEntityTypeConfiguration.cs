using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Infrastructure.EntityConfigurations
{
    public class CatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("CatalogItem", CatalogContext.DEFAULT_SCHEMA);


            builder.Property(ci => ci.Id)
                .UseHiLo("catalog_hilo")
                .IsRequired();

            builder.Property(ci => ci.Name)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(ci => ci.Price)
                .IsRequired(true);

            builder.Property(ci => ci.PictureFileName)
                .IsRequired(false);

            builder.HasOne(ci=>ci.CatalogBrand)
                .WithMany()
                .HasForeignKey(ci=>ci.CatalogBrandId);

            builder.HasOne(ci=>ci.CatalogType)
                .WithMany()
                .HasForeignKey(ci=>ci.CatalogTypeId);
        }
    }
}