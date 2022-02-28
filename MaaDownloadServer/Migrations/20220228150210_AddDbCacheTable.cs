using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaDownloadServer.Migrations
{
    public partial class AddDbCacheTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "database_cache",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    query_id = table.Column<string>(type: "TEXT", nullable: true),
                    value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_database_cache", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "database_cache");
        }
    }
}
