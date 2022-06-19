using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesDataService_API.Migrations
{
    public partial class AddedIsAvailableGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Games");
        }
    }
}
