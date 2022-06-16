using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameAccessService_API.Migrations
{
    public partial class ChangeUserAccessData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedBy",
                table: "UsersGameAccess");

            migrationBuilder.RenameColumn(
                name: "ExpireDate",
                table: "UsersGameAccess",
                newName: "BanExpires");

            migrationBuilder.RenameColumn(
                name: "BlockDate",
                table: "UsersGameAccess",
                newName: "BanDate");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "UsersGameAccess",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BanExpires",
                table: "UsersGameAccess",
                newName: "ExpireDate");

            migrationBuilder.RenameColumn(
                name: "BanDate",
                table: "UsersGameAccess",
                newName: "BlockDate");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "UsersGameAccess",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BlockedBy",
                table: "UsersGameAccess",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
