using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsService_API.Migrations
{
    public partial class UpdateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GamesProducts_GameId",
                table: "GamesProducts",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_GamesProducts_Games_GameId",
                table: "GamesProducts",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamesProducts_Games_GameId",
                table: "GamesProducts");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropIndex(
                name: "IX_GamesProducts_GameId",
                table: "GamesProducts");
        }
    }
}
