using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductionReadyApi.Domain.Entities;

namespace ProductionReadyApi.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(product => product.Id)
            .HasName("pk_products");

        builder.Property(product => product.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(product => product.Sku)
            .HasColumnName("sku")
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(product => product.Sku)
            .IsUnique()
            .HasDatabaseName("ux_products_sku");

        builder.Property(product => product.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.Price)
            .HasColumnName("price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(product => product.StockQuantity)
            .HasColumnName("stock_quantity")
            .IsRequired();

        builder.Property(product => product.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(product => product.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
