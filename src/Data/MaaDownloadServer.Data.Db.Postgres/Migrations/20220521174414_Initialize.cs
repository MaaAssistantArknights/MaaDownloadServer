using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaaDownloadServer.Data.Db.Postgres.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blobs",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Sha1 = table.Column<string>(type: "text", nullable: false),
                    Md5 = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blobs", x => x.EntityId);
                });

            migrationBuilder.CreateTable(
                name: "ExternalModules",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalModules", x => x.EntityId);
                });

            migrationBuilder.CreateTable(
                name: "MaaModules",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaaModules", x => x.EntityId);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlobEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    DownloadCount = table.Column<int>(type: "integer", nullable: false),
                    IsBundle = table.Column<bool>(type: "boolean", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Assets_Blobs_BlobEntityId",
                        column: x => x.BlobEntityId,
                        principalTable: "Blobs",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaaVersions",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaaModuleEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    UpdateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ChangeLog = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaaVersions", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_MaaVersions_MaaModules_MaaModuleEntityId",
                        column: x => x.MaaModuleEntityId,
                        principalTable: "MaaModules",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalSyncStatus",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalModuleEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastSync = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AssetEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalSyncStatus", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_ExternalSyncStatus_Assets_AssetEntityId",
                        column: x => x.AssetEntityId,
                        principalTable: "Assets",
                        principalColumn: "EntityId");
                    table.ForeignKey(
                        name: "FK_ExternalSyncStatus_ExternalModules_ExternalModuleEntityId",
                        column: x => x.ExternalModuleEntityId,
                        principalTable: "ExternalModules",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Map_File_Asset",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlobId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetsEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    FilesEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Map_File_Asset", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Map_File_Asset_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_File_Asset_Assets_AssetsEntityId",
                        column: x => x.AssetsEntityId,
                        principalTable: "Assets",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_File_Asset_Blobs_BlobId",
                        column: x => x.BlobId,
                        principalTable: "Blobs",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_File_Asset_Blobs_FilesEntityId",
                        column: x => x.FilesEntityId,
                        principalTable: "Blobs",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaaPackages",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaaModuleEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaaVersionEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaaPackages", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_MaaPackages_Assets_AssetEntityId",
                        column: x => x.AssetEntityId,
                        principalTable: "Assets",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaaPackages_MaaModules_MaaModuleEntityId",
                        column: x => x.MaaModuleEntityId,
                        principalTable: "MaaModules",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaaPackages_MaaVersions_MaaVersionEntityId",
                        column: x => x.MaaVersionEntityId,
                        principalTable: "MaaVersions",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaaSyncStatus",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaaModuleEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastSync = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LatestVersionEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaaSyncStatus", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_MaaSyncStatus_MaaModules_MaaModuleEntityId",
                        column: x => x.MaaModuleEntityId,
                        principalTable: "MaaModules",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaaSyncStatus_MaaVersions_LatestVersionEntityId",
                        column: x => x.LatestVersionEntityId,
                        principalTable: "MaaVersions",
                        principalColumn: "EntityId");
                });

            migrationBuilder.CreateTable(
                name: "MaaUpdatePackages",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaaModuleEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionToEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionFromEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaaUpdatePackages", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_MaaUpdatePackages_Assets_AssetEntityId",
                        column: x => x.AssetEntityId,
                        principalTable: "Assets",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaaUpdatePackages_MaaModules_MaaModuleEntityId",
                        column: x => x.MaaModuleEntityId,
                        principalTable: "MaaModules",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaaUpdatePackages_MaaVersions_VersionFromEntityId",
                        column: x => x.VersionFromEntityId,
                        principalTable: "MaaVersions",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaaUpdatePackages_MaaVersions_VersionToEntityId",
                        column: x => x.VersionToEntityId,
                        principalTable: "MaaVersions",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Map_Blob_MaaUpAdd",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlobId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaaUpdateId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaaUpAddEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Map_Blob_MaaUpAdd", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpAdd_Blobs_AddEntityId",
                        column: x => x.AddEntityId,
                        principalTable: "Blobs",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpAdd_Blobs_BlobId",
                        column: x => x.BlobId,
                        principalTable: "Blobs",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpAdd_MaaUpdatePackages_MaaUpAddEntityId",
                        column: x => x.MaaUpAddEntityId,
                        principalTable: "MaaUpdatePackages",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpAdd_MaaUpdatePackages_MaaUpdateId",
                        column: x => x.MaaUpdateId,
                        principalTable: "MaaUpdatePackages",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Map_Blob_MaaUpRemove",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlobId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaaUpdateId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaaUpRemoveEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    RemoveEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Map_Blob_MaaUpRemove", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpRemove_Blobs_BlobId",
                        column: x => x.BlobId,
                        principalTable: "Blobs",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpRemove_Blobs_RemoveEntityId",
                        column: x => x.RemoveEntityId,
                        principalTable: "Blobs",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpRemove_MaaUpdatePackages_MaaUpdateId",
                        column: x => x.MaaUpdateId,
                        principalTable: "MaaUpdatePackages",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpRemove_MaaUpdatePackages_MaaUpRemoveEntityId",
                        column: x => x.MaaUpRemoveEntityId,
                        principalTable: "MaaUpdatePackages",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Map_Blob_MaaUpUpdate",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlobId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaaUpdateId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaaUpUpdateEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdateEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Map_Blob_MaaUpUpdate", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpUpdate_Blobs_BlobId",
                        column: x => x.BlobId,
                        principalTable: "Blobs",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpUpdate_Blobs_UpdateEntityId",
                        column: x => x.UpdateEntityId,
                        principalTable: "Blobs",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpUpdate_MaaUpdatePackages_MaaUpdateId",
                        column: x => x.MaaUpdateId,
                        principalTable: "MaaUpdatePackages",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Map_Blob_MaaUpUpdate_MaaUpdatePackages_MaaUpUpdateEntityId",
                        column: x => x.MaaUpUpdateEntityId,
                        principalTable: "MaaUpdatePackages",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_BlobEntityId",
                table: "Assets",
                column: "BlobEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalSyncStatus_AssetEntityId",
                table: "ExternalSyncStatus",
                column: "AssetEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalSyncStatus_ExternalModuleEntityId",
                table: "ExternalSyncStatus",
                column: "ExternalModuleEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaaPackages_AssetEntityId",
                table: "MaaPackages",
                column: "AssetEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaaPackages_MaaModuleEntityId",
                table: "MaaPackages",
                column: "MaaModuleEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaaPackages_MaaVersionEntityId",
                table: "MaaPackages",
                column: "MaaVersionEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaaSyncStatus_LatestVersionEntityId",
                table: "MaaSyncStatus",
                column: "LatestVersionEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaaSyncStatus_MaaModuleEntityId",
                table: "MaaSyncStatus",
                column: "MaaModuleEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaaUpdatePackages_AssetEntityId",
                table: "MaaUpdatePackages",
                column: "AssetEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaaUpdatePackages_MaaModuleEntityId",
                table: "MaaUpdatePackages",
                column: "MaaModuleEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaaUpdatePackages_VersionFromEntityId",
                table: "MaaUpdatePackages",
                column: "VersionFromEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaaUpdatePackages_VersionToEntityId",
                table: "MaaUpdatePackages",
                column: "VersionToEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MaaVersions_MaaModuleEntityId",
                table: "MaaVersions",
                column: "MaaModuleEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpAdd_AddEntityId",
                table: "Map_Blob_MaaUpAdd",
                column: "AddEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpAdd_BlobId",
                table: "Map_Blob_MaaUpAdd",
                column: "BlobId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpAdd_MaaUpAddEntityId",
                table: "Map_Blob_MaaUpAdd",
                column: "MaaUpAddEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpAdd_MaaUpdateId",
                table: "Map_Blob_MaaUpAdd",
                column: "MaaUpdateId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpRemove_BlobId",
                table: "Map_Blob_MaaUpRemove",
                column: "BlobId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpRemove_MaaUpdateId",
                table: "Map_Blob_MaaUpRemove",
                column: "MaaUpdateId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpRemove_MaaUpRemoveEntityId",
                table: "Map_Blob_MaaUpRemove",
                column: "MaaUpRemoveEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpRemove_RemoveEntityId",
                table: "Map_Blob_MaaUpRemove",
                column: "RemoveEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpUpdate_BlobId",
                table: "Map_Blob_MaaUpUpdate",
                column: "BlobId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpUpdate_MaaUpdateId",
                table: "Map_Blob_MaaUpUpdate",
                column: "MaaUpdateId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpUpdate_MaaUpUpdateEntityId",
                table: "Map_Blob_MaaUpUpdate",
                column: "MaaUpUpdateEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_Blob_MaaUpUpdate_UpdateEntityId",
                table: "Map_Blob_MaaUpUpdate",
                column: "UpdateEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_File_Asset_AssetId",
                table: "Map_File_Asset",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_File_Asset_AssetsEntityId",
                table: "Map_File_Asset",
                column: "AssetsEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_File_Asset_BlobId",
                table: "Map_File_Asset",
                column: "BlobId");

            migrationBuilder.CreateIndex(
                name: "IX_Map_File_Asset_FilesEntityId",
                table: "Map_File_Asset",
                column: "FilesEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalSyncStatus");

            migrationBuilder.DropTable(
                name: "MaaPackages");

            migrationBuilder.DropTable(
                name: "MaaSyncStatus");

            migrationBuilder.DropTable(
                name: "Map_Blob_MaaUpAdd");

            migrationBuilder.DropTable(
                name: "Map_Blob_MaaUpRemove");

            migrationBuilder.DropTable(
                name: "Map_Blob_MaaUpUpdate");

            migrationBuilder.DropTable(
                name: "Map_File_Asset");

            migrationBuilder.DropTable(
                name: "ExternalModules");

            migrationBuilder.DropTable(
                name: "MaaUpdatePackages");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "MaaVersions");

            migrationBuilder.DropTable(
                name: "Blobs");

            migrationBuilder.DropTable(
                name: "MaaModules");
        }
    }
}
