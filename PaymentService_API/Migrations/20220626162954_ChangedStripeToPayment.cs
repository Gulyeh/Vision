using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService_API.Migrations
{
    public partial class ChangedStripeToPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StripeUrl",
                table: "Payments",
                newName: "PaymentUrl");

            migrationBuilder.RenameColumn(
                name: "StripeId",
                table: "Payments",
                newName: "PaymentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentUrl",
                table: "Payments",
                newName: "StripeUrl");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "Payments",
                newName: "StripeId");
        }
    }
}
