using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService_API.Migrations
{
    public partial class AddedCurrencyToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedBy",
                table: "UsersFriends");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyValue",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyValue",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "BlockedBy",
                table: "UsersFriends",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
