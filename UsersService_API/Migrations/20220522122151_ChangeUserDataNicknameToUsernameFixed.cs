using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService_API.Migrations
{
    public partial class ChangeUserDataNicknameToUsernameFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nickname",
                table: "Users",
                newName: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "Nickname");
        }
    }
}
