using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductionReadyApi.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "products",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                sku = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                stock_quantity = table.Column<int>(type: "integer", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_products", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ux_products_sku",
            table: "products",
            column: "sku",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "products");
    }
}
