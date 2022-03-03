using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaDownloadServer.Migrations
{
    public partial class UpdatePenguinStatCacheTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "en_zone_name",
                table: "ark_penguin_stage");

            migrationBuilder.DropColumn(
                name: "ja_zone_name",
                table: "ark_penguin_stage");

            migrationBuilder.DropColumn(
                name: "ko_zone_name",
                table: "ark_penguin_stage");

            migrationBuilder.DropColumn(
                name: "zh_zone_name",
                table: "ark_penguin_stage");

            migrationBuilder.DropColumn(
                name: "zone_id",
                table: "ark_penguin_stage");

            migrationBuilder.DropColumn(
                name: "zone_name",
                table: "ark_penguin_stage");

            migrationBuilder.RenameColumn(
                name: "zone_type",
                table: "ark_penguin_stage",
                newName: "ArkPenguinZoneZoneId");

            migrationBuilder.AddColumn<long>(
                name: "min_clear_time",
                table: "ark_penguin_stage",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

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
                    ko_name = table.Column<string>(type: "TEXT", nullable: true),
                    ArkPenguinStageStageId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ark_penguin_item", x => x.item_id);
                    table.ForeignKey(
                        name: "FK_ark_penguin_item_ark_penguin_stage_ArkPenguinStageStageId",
                        column: x => x.ArkPenguinStageStageId,
                        principalTable: "ark_penguin_stage",
                        principalColumn: "stage_id");
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

            migrationBuilder.CreateIndex(
                name: "IX_ark_penguin_stage_ArkPenguinZoneZoneId",
                table: "ark_penguin_stage",
                column: "ArkPenguinZoneZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ark_penguin_item_ArkPenguinStageStageId",
                table: "ark_penguin_item",
                column: "ArkPenguinStageStageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ark_penguin_stage_ark_penguin_zone_ArkPenguinZoneZoneId",
                table: "ark_penguin_stage",
                column: "ArkPenguinZoneZoneId",
                principalTable: "ark_penguin_zone",
                principalColumn: "zone_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ark_penguin_stage_ark_penguin_zone_ArkPenguinZoneZoneId",
                table: "ark_penguin_stage");

            migrationBuilder.DropTable(
                name: "ark_penguin_item");

            migrationBuilder.DropTable(
                name: "ark_penguin_zone");

            migrationBuilder.DropIndex(
                name: "IX_ark_penguin_stage_ArkPenguinZoneZoneId",
                table: "ark_penguin_stage");

            migrationBuilder.DropColumn(
                name: "min_clear_time",
                table: "ark_penguin_stage");

            migrationBuilder.RenameColumn(
                name: "ArkPenguinZoneZoneId",
                table: "ark_penguin_stage",
                newName: "zone_type");

            migrationBuilder.AddColumn<string>(
                name: "en_zone_name",
                table: "ark_penguin_stage",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ja_zone_name",
                table: "ark_penguin_stage",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ko_zone_name",
                table: "ark_penguin_stage",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "zh_zone_name",
                table: "ark_penguin_stage",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "zone_id",
                table: "ark_penguin_stage",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "zone_name",
                table: "ark_penguin_stage",
                type: "TEXT",
                nullable: true);
        }
    }
}
