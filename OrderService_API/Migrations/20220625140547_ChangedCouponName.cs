using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService_API.Migrations
{
    public partial class ChangedCouponName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CuponCode",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "CuponCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
