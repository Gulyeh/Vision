using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodesService_API.Migrations
{
    public partial class ChangeCodesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CodeType",
                table: "Codes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Uses",
                table: "Codes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "gameId",
                table: "Codes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isLimited",
                table: "Codes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeType",
                table: "Codes");

            migrationBuilder.DropColumn(
                name: "Uses",
                table: "Codes");

            migrationBuilder.DropColumn(
                name: "gameId",
                table: "Codes");

            migrationBuilder.DropColumn(
                name: "isLimited",
                table: "Codes");
        }
    }
}
