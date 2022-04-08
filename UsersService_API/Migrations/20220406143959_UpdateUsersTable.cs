using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService_API.Migrations
{
    public partial class UpdateUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastOnlineStatus",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastOnlineStatus",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Users");
        }
    }
}
