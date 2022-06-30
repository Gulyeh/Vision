using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService_API.Migrations
{
    public partial class AddedPaymentIsAvailable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "PaymentMethods",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "PaymentMethods");
        }
    }
}
