using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaDownloadServer.Data.Db.Postgres.Migrations
{
    public partial class AddAssetDownloadUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetDownloadUrl",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    AssetEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDownloadUrl", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_AssetDownloadUrl_Assets_AssetEntityId",
                        column: x => x.AssetEntityId,
                        principalTable: "Assets",
                        principalColumn: "EntityId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetDownloadUrl_AssetEntityId",
                table: "AssetDownloadUrl",
                column: "AssetEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetDownloadUrl");
        }
    }
}
