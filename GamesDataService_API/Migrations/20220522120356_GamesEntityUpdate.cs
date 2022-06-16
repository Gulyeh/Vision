using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesDataService_API.Migrations
{
    public partial class GamesEntityUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requirements_GameId",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Informations_GameId",
                table: "Informations");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_GameId",
                table: "Requirements",
                column: "GameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Informations_GameId",
                table: "Informations",
                column: "GameId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requirements_GameId",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Informations_GameId",
                table: "Informations");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_GameId",
                table: "Requirements",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Informations_GameId",
                table: "Informations",
                column: "GameId");
        }
    }
}
