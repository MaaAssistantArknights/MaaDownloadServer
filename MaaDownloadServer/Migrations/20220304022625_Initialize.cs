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
                name: "ark_penguin_item",
                columns: table => new
                {
                    item_id = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    sort_id = table.Column<int>(type: "INTEGER", nullable: false),
                    rarity = table.Column<int>(type: "INTEGER", nullable: false),
                    item_type = table.Column<string>(type: "TEXT", nullable: true),
                    us_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    jp_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    kr_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    cn_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    zh_name = table.Column<string>(type: "TEXT", nullable: true),
                    en_name = table.Column<string>(type: "TEXT", nullable: true),
                    jp_name = table.Column<string>(type: "TEXT", nullable: true),
                    ko_name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ark_penguin_item", x => x.item_id);
                });

            migrationBuilder.CreateTable(
                name: "ark_penguin_zone",
                columns: table => new
                {
                    zone_id = table.Column<string>(type: "TEXT", nullable: false),
                    zone_name = table.Column<string>(type: "TEXT", nullable: true),
                    zone_type = table.Column<string>(type: "TEXT", nullable: true),
                    background = table.Column<string>(type: "TEXT", nullable: true),
                    background_file_name = table.Column<string>(type: "TEXT", nullable: true),
                    us_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    jp_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    kr_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    cn_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    ko_zone_name = table.Column<string>(type: "TEXT", nullable: true),
                    ja_zone_name = table.Column<string>(type: "TEXT", nullable: true),
                    en_zone_name = table.Column<string>(type: "TEXT", nullable: true),
                    zh_zone_name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ark_penguin_zone", x => x.zone_id);
                });

            migrationBuilder.CreateTable(
                name: "ark_prts_item",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    item_id = table.Column<string>(type: "TEXT", nullable: true),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    usage = table.Column<string>(type: "TEXT", nullable: true),
                    obtain = table.Column<string>(type: "TEXT", nullable: true),
                    rarity = table.Column<int>(type: "INTEGER", nullable: false),
                    image = table.Column<string>(type: "TEXT", nullable: true),
                    image_download_url = table.Column<string>(type: "TEXT", nullable: true),
                    category = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ark_prts_item", x => x.id);
                });

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

            migrationBuilder.CreateTable(
                name: "package",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    version = table.Column<string>(type: "TEXT", nullable: true),
                    platform = table.Column<string>(type: "TEXT", nullable: false),
                    architecture = table.Column<string>(type: "TEXT", nullable: false),
                    publish_time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    update_log = table.Column<string>(type: "TEXT", nullable: true),
                    component = table.Column<string>(type: "TEXT", nullable: true)
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
                    file_extension = table.Column<string>(type: "TEXT", nullable: true),
                    tag = table.Column<string>(type: "TEXT", nullable: true),
                    duration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    hash = table.Column<string>(type: "TEXT", nullable: true),
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
                    file_name = table.Column<string>(type: "TEXT", nullable: true),
                    path = table.Column<string>(type: "TEXT", nullable: true),
                    hash = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ark_penguin_stage",
                columns: table => new
                {
                    stage_id = table.Column<string>(type: "TEXT", nullable: false),
                    stage_type = table.Column<string>(type: "TEXT", nullable: true),
                    stage_code = table.Column<string>(type: "TEXT", nullable: true),
                    stage_ap_cost = table.Column<int>(type: "INTEGER", nullable: false),
                    min_clear_time = table.Column<long>(type: "INTEGER", nullable: false),
                    us_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    jp_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    kr_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    cn_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    ko_stage_code = table.Column<string>(type: "TEXT", nullable: true),
                    ja_stage_code = table.Column<string>(type: "TEXT", nullable: true),
                    en_stage_code = table.Column<string>(type: "TEXT", nullable: true),
                    zh_stage_code = table.Column<string>(type: "TEXT", nullable: true),
                    us_open_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    us_close_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    jp_open_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    jp_close_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    kr_open_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    kr_close_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    cn_open_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    cn_close_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    drop_items = table.Column<string>(type: "TEXT", nullable: true),
                    ArkPenguinZoneZoneId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ark_penguin_stage", x => x.stage_id);
                    table.ForeignKey(
                        name: "FK_ark_penguin_stage_ark_penguin_zone_ArkPenguinZoneZoneId",
                        column: x => x.ArkPenguinZoneZoneId,
                        principalTable: "ark_penguin_zone",
                        principalColumn: "zone_id");
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
                name: "IX_ark_penguin_stage_ArkPenguinZoneZoneId",
                table: "ark_penguin_stage",
                column: "ArkPenguinZoneZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageResource_ResourcesId",
                table: "PackageResource",
                column: "ResourcesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ark_penguin_item");

            migrationBuilder.DropTable(
                name: "ark_penguin_stage");

            migrationBuilder.DropTable(
                name: "ark_prts_item");

            migrationBuilder.DropTable(
                name: "database_cache");

            migrationBuilder.DropTable(
                name: "PackageResource");

            migrationBuilder.DropTable(
                name: "public_content");

            migrationBuilder.DropTable(
                name: "ark_penguin_zone");

            migrationBuilder.DropTable(
                name: "package");

            migrationBuilder.DropTable(
                name: "resource");
        }
    }
}
