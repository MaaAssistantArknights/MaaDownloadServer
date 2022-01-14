using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaDownloadServer.Migrations
{
    public partial class AddPublishTimeColume : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PublishTime",
                table: "Packages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishTime",
                table: "Packages");
        }
    }
}
