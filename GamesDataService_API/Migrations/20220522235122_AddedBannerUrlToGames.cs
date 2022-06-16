using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesDataService_API.Migrations
{
    public partial class AddedBannerUrlToGames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerId",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "Games");
        }
    }
}
