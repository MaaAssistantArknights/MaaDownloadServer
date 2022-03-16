using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaDownloadServer.Migrations
{
    public partial class RemoveGameData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ark_penguin_item");

            migrationBuilder.DropTable(
                name: "ark_penguin_stage");

            migrationBuilder.DropTable(
                name: "ark_prts_item");

            migrationBuilder.DropTable(
                name: "ark_penguin_zone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ark_penguin_item",
                columns: table => new
                {
                    item_id = table.Column<string>(type: "TEXT", nullable: false),
                    cn_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    en_name = table.Column<string>(type: "TEXT", nullable: true),
                    item_type = table.Column<string>(type: "TEXT", nullable: true),
                    ja_name = table.Column<string>(type: "TEXT", nullable: true),
                    jp_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    ko_name = table.Column<string>(type: "TEXT", nullable: true),
                    kr_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    rarity = table.Column<int>(type: "INTEGER", nullable: false),
                    sort_id = table.Column<int>(type: "INTEGER", nullable: false),
                    us_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    zh_name = table.Column<string>(type: "TEXT", nullable: true)
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
                    background = table.Column<string>(type: "TEXT", nullable: true),
                    background_file_name = table.Column<string>(type: "TEXT", nullable: true),
                    cn_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    en_zone_name = table.Column<string>(type: "TEXT", nullable: true),
                    ja_zone_name = table.Column<string>(type: "TEXT", nullable: true),
                    jp_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    ko_zone_name = table.Column<string>(type: "TEXT", nullable: true),
                    kr_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    us_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    zh_zone_name = table.Column<string>(type: "TEXT", nullable: true),
                    zone_name = table.Column<string>(type: "TEXT", nullable: true),
                    zone_type = table.Column<string>(type: "TEXT", nullable: true)
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
                    category = table.Column<string>(type: "TEXT", nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    image = table.Column<string>(type: "TEXT", nullable: true),
                    image_download_url = table.Column<string>(type: "TEXT", nullable: true),
                    item_id = table.Column<string>(type: "TEXT", nullable: true),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    obtain = table.Column<string>(type: "TEXT", nullable: true),
                    rarity = table.Column<int>(type: "INTEGER", nullable: false),
                    usage = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ark_prts_item", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ark_penguin_stage",
                columns: table => new
                {
                    stage_id = table.Column<string>(type: "TEXT", nullable: false),
                    ArkPenguinZoneZoneId = table.Column<string>(type: "TEXT", nullable: true),
                    cn_close_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    cn_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    cn_open_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    drop_items = table.Column<string>(type: "TEXT", nullable: true),
                    en_stage_code = table.Column<string>(type: "TEXT", nullable: true),
                    ja_stage_code = table.Column<string>(type: "TEXT", nullable: true),
                    jp_close_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    jp_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    jp_open_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ko_stage_code = table.Column<string>(type: "TEXT", nullable: true),
                    kr_close_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    kr_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    kr_open_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    min_clear_time = table.Column<long>(type: "INTEGER", nullable: false),
                    stage_ap_cost = table.Column<int>(type: "INTEGER", nullable: false),
                    stage_code = table.Column<string>(type: "TEXT", nullable: true),
                    stage_type = table.Column<string>(type: "TEXT", nullable: true),
                    us_close_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    us_exist = table.Column<bool>(type: "INTEGER", nullable: false),
                    us_open_time = table.Column<DateTime>(type: "TEXT", nullable: true),
                    zh_stage_code = table.Column<string>(type: "TEXT", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_ark_penguin_stage_ArkPenguinZoneZoneId",
                table: "ark_penguin_stage",
                column: "ArkPenguinZoneZoneId");
        }
    }
}
