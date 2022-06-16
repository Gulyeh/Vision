using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsService_API.Migrations
{
    public partial class RemoveOneTimePurchase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOneTimePurchase",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOneTimePurchase",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
