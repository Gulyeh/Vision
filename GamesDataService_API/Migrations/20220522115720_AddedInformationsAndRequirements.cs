using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesDataService_API.Migrations
{
    public partial class AddedInformationsAndRequirements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Informations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Developer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Publisher = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Informations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Informations_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinimumOS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumMemory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumCPU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumGPU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinimumStorage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedOS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedMemory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedCPU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedGPU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedStorage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requirements_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Informations_GameId",
                table: "Informations",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_GameId",
                table: "Requirements",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Informations");

            migrationBuilder.DropTable(
                name: "Requirements");
        }
    }
}
