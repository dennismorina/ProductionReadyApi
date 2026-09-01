using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ProductionReadyApi.Infrastructure.Persistence;

#nullable disable

namespace ProductionReadyApi.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("202609010001_InitialCreate")]
partial class InitialCreate
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.11")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity("ProductionReadyApi.Domain.Entities.Product", entity =>
        {
            entity.Property<Guid>("Id")
                .ValueGeneratedNever()
                .HasColumnType("uuid")
                .HasColumnName("id");

            entity.Property<DateTimeOffset>("CreatedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");

            entity.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("character varying(200)")
                .HasColumnName("name");

            entity.Property<decimal>("Price")
                .HasPrecision(18, 2)
                .HasColumnType("numeric(18,2)")
                .HasColumnName("price");

            entity.Property<string>("Sku")
                .IsRequired()
                .HasMaxLength(64)
                .HasColumnType("character varying(64)")
                .HasColumnName("sku");

            entity.Property<int>("StockQuantity")
                .HasColumnType("integer")
                .HasColumnName("stock_quantity");

            entity.Property<DateTimeOffset>("UpdatedAt")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");

            entity.HasKey("Id")
                .HasName("pk_products");

            entity.HasIndex("Sku")
                .IsUnique()
                .HasDatabaseName("ux_products_sku");

            entity.ToTable("products", (string)null);
        });
#pragma warning restore 612, 618
    }
}
