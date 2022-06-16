using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodesService_API.Migrations
{
    public partial class ChangeCodeTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isLimited",
                table: "Codes",
                newName: "IsLimited");

            migrationBuilder.RenameColumn(
                name: "gameId",
                table: "Codes",
                newName: "GameId");

            migrationBuilder.AlterColumn<string>(
                name: "CodeValue",
                table: "Codes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "Codes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature",
                table: "Codes");

            migrationBuilder.RenameColumn(
                name: "IsLimited",
                table: "Codes",
                newName: "isLimited");

            migrationBuilder.RenameColumn(
                name: "GameId",
                table: "Codes",
                newName: "gameId");

            migrationBuilder.AlterColumn<string>(
                name: "CodeValue",
                table: "Codes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
