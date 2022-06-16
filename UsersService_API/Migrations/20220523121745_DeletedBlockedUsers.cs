using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService_API.Migrations
{
    public partial class DeletedBlockedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "UsersFriends");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "UsersFriends",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
