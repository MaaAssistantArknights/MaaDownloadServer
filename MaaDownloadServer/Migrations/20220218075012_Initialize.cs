using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaDownloadServer.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ark_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    item_id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: false),
                    usage = table.Column<string>(type: "TEXT", nullable: false),
                    obtain_approach = table.Column<string>(type: "TEXT", nullable: false),
                    rarity = table.Column<int>(type: "INTEGER", nullable: false),
                    image = table.Column<string>(type: "TEXT", nullable: false),
                    category = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ark_item", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ark_stage",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    zone_id = table.Column<string>(type: "TEXT", nullable: false),
                    stage_id = table.Column<string>(type: "TEXT", nullable: false),
                    zone_name = table.Column<string>(type: "TEXT", nullable: false),
                    stage_name = table.Column<string>(type: "TEXT", nullable: false),
                    sanity_cost = table.Column<int>(type: "INTEGER", nullable: false),
                    open_cn = table.Column<string>(type: "TEXT", nullable: true),
                    close_cn = table.Column<string>(type: "TEXT", nullable: true),
                    exist_cn = table.Column<bool>(type: "INTEGER", nullable: false),
                    open_global = table.Column<string>(type: "TEXT", nullable: true),
                    close_global = table.Column<string>(type: "TEXT", nullable: true),
                    exist_global = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ark_stage", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "package",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    version = table.Column<string>(type: "TEXT", nullable: false),
                    platform = table.Column<string>(type: "TEXT", nullable: false),
                    architecture = table.Column<string>(type: "TEXT", nullable: false),
                    publish_time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    update_log = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_package", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "public_content",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    tag = table.Column<string>(type: "TEXT", nullable: false),
                    duration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    hash = table.Column<string>(type: "TEXT", nullable: false),
                    add_time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_public_content", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "resource",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    file_name = table.Column<string>(type: "TEXT", nullable: false),
                    path = table.Column<string>(type: "TEXT", nullable: false),
                    hash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PackageResource",
                columns: table => new
                {
                    PackagesId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResourcesId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageResource", x => new { x.PackagesId, x.ResourcesId });
                    table.ForeignKey(
                        name: "FK_PackageResource_package_PackagesId",
                        column: x => x.PackagesId,
                        principalTable: "package",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageResource_resource_ResourcesId",
                        column: x => x.ResourcesId,
                        principalTable: "resource",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageResource_ResourcesId",
                table: "PackageResource",
                column: "ResourcesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ark_item");

            migrationBuilder.DropTable(
                name: "ark_stage");

            migrationBuilder.DropTable(
                name: "PackageResource");

            migrationBuilder.DropTable(
                name: "public_content");

            migrationBuilder.DropTable(
                name: "package");

            migrationBuilder.DropTable(
                name: "resource");
        }
    }
}
