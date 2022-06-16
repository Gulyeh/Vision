using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersService_API.Migrations
{
    public partial class AddedIsDeletedAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeletedAccount",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeletedAccount",
                table: "Users");
        }
    }
}
