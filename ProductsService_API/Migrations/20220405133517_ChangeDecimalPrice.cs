using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsService_API.Migrations
{
    public partial class ChangeDecimalPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "GamesProducts",
                type: "decimal(15,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "GamesProducts",
                type: "decimal(4,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(15,2)");
        }
    }
}
