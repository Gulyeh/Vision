using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodesService_API.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Codes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CodeValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CodeType = table.Column<int>(type: "int", nullable: false),
                    Signature = table.Column<int>(type: "int", nullable: true),
                    IsLimited = table.Column<bool>(type: "bit", nullable: false),
                    Uses = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Codes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CodesUsed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodesUsed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodesUsed_Codes_CodeId",
                        column: x => x.CodeId,
                        principalTable: "Codes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Codes_Code",
                table: "Codes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodesUsed_CodeId",
                table: "CodesUsed",
                column: "CodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodesUsed");

            migrationBuilder.DropTable(
                name: "Codes");
        }
    }
}
