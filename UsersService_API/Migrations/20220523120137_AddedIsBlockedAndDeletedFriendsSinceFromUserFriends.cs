using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService_API.Migrations
{
    public partial class AddedIsBlockedAndDeletedFriendsSinceFromUserFriends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendsSince",
                table: "UsersFriends");

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "UsersFriends",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "UsersFriends");

            migrationBuilder.AddColumn<DateTime>(
                name: "FriendsSince",
                table: "UsersFriends",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
