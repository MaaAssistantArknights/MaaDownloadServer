using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaDownloadServer.Migrations
{
    public partial class SupportComponents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "file_extension",
                table: "public_content",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "component",
                table: "package",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_extension",
                table: "public_content");

            migrationBuilder.DropColumn(
                name: "component",
                table: "package");
        }
    }
}
