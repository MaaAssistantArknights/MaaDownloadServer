using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaDownloadServer.Migrations
{
    public partial class AddDownloadCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "download_count",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    component_name = table.Column<string>(type: "TEXT", nullable: true),
                    from_version = table.Column<string>(type: "TEXT", nullable: true),
                    to_version = table.Column<string>(type: "TEXT", nullable: true),
                    count = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_download_count", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "download_count");
        }
    }
}
