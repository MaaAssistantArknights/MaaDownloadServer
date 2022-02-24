﻿// <auto-generated />
using System;
using MaaDownloadServer.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MaaDownloadServer.Migrations
{
    [DbContext(typeof(MaaDownloadServerDbContext))]
    [Migration("20220223131403_Initialize")]
    partial class Initialize
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("MaaDownloadServer.Model.Entities.ArkPenguinStage", b =>
                {
                    b.Property<string>("StageId")
                        .HasColumnType("TEXT")
                        .HasColumnName("stage_id");

                    b.Property<DateTime?>("CnCloseTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("cn_close_time");

                    b.Property<bool>("CnExist")
                        .HasColumnType("INTEGER")
                        .HasColumnName("cn_exist");

                    b.Property<DateTime?>("CnOpenTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("cn_open_time");

                    b.Property<string>("EnStageCodeI18N")
                        .HasColumnType("TEXT")
                        .HasColumnName("en_stage_code");

                    b.Property<string>("EnZoneNameI18N")
                        .HasColumnType("TEXT")
                        .HasColumnName("en_zone_name");

                    b.Property<string>("JaStageCodeI18N")
                        .HasColumnType("TEXT")
                        .HasColumnName("ja_stage_code");

                    b.Property<string>("JaZoneNameI18N")
                        .HasColumnType("TEXT")
                        .HasColumnName("ja_zone_name");

                    b.Property<DateTime?>("JpCloseTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("jp_close_time");

                    b.Property<bool>("JpExist")
                        .HasColumnType("INTEGER")
                        .HasColumnName("jp_exist");

                    b.Property<DateTime?>("JpOpenTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("jp_open_time");

                    b.Property<string>("KoStageCodeI18N")
                        .HasColumnType("TEXT")
                        .HasColumnName("ko_stage_code");

                    b.Property<string>("KoZoneNameI18N")
                        .HasColumnType("TEXT")
                        .HasColumnName("ko_zone_name");

                    b.Property<DateTime?>("KrCloseTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("kr_close_time");

                    b.Property<bool>("KrExist")
                        .HasColumnType("INTEGER")
                        .HasColumnName("kr_exist");

                    b.Property<DateTime?>("KrOpenTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("kr_open_time");

                    b.Property<int>("StageApCost")
                        .HasColumnType("INTEGER")
                        .HasColumnName("stage_ap_cost");

                    b.Property<string>("StageCode")
                        .HasColumnType("TEXT")
                        .HasColumnName("stage_code");

                    b.Property<string>("StageType")
                        .HasColumnType("TEXT")
                        .HasColumnName("stage_type");

                    b.Property<DateTime?>("UsCloseTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("us_close_time");

                    b.Property<bool>("UsExist")
                        .HasColumnType("INTEGER")
                        .HasColumnName("us_exist");

                    b.Property<DateTime?>("UsOpenTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("us_open_time");

                    b.Property<string>("ZhStageCodeI18N")
                        .HasColumnType("TEXT")
                        .HasColumnName("zh_stage_code");

                    b.Property<string>("ZhZoneNameI18N")
                        .HasColumnType("TEXT")
                        .HasColumnName("zh_zone_name");

                    b.Property<string>("ZoneId")
                        .HasColumnType("TEXT")
                        .HasColumnName("zone_id");

                    b.Property<string>("ZoneName")
                        .HasColumnType("TEXT")
                        .HasColumnName("zone_name");

                    b.Property<string>("ZoneType")
                        .HasColumnType("TEXT")
                        .HasColumnName("zone_type");

                    b.HasKey("StageId");

                    b.ToTable("ark_penguin_stage");
                });

            modelBuilder.Entity("MaaDownloadServer.Model.Entities.ArkPrtsItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("Category")
                        .HasColumnType("TEXT")
                        .HasColumnName("category");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasColumnName("description");

                    b.Property<string>("Image")
                        .HasColumnType("TEXT")
                        .HasColumnName("image");

                    b.Property<string>("ImageDownloadUrl")
                        .HasColumnType("TEXT")
                        .HasColumnName("image_download_url");

                    b.Property<string>("ItemId")
                        .HasColumnType("TEXT")
                        .HasColumnName("item_id");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("ObtainMethod")
                        .HasColumnType("TEXT")
                        .HasColumnName("obtain");

                    b.Property<int>("Rarity")
                        .HasColumnType("INTEGER")
                        .HasColumnName("rarity");

                    b.Property<string>("Usage")
                        .HasColumnType("TEXT")
                        .HasColumnName("usage");

                    b.HasKey("Id");

                    b.ToTable("ark_prts_item");
                });

            modelBuilder.Entity("MaaDownloadServer.Model.Entities.Package", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("Architecture")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("architecture");

                    b.Property<string>("Component")
                        .HasColumnType("TEXT")
                        .HasColumnName("component");

                    b.Property<string>("Platform")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("platform");

                    b.Property<DateTime>("PublishTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("publish_time");

                    b.Property<string>("UpdateLog")
                        .HasColumnType("TEXT")
                        .HasColumnName("update_log");

                    b.Property<string>("Version")
                        .HasColumnType("TEXT")
                        .HasColumnName("version");

                    b.HasKey("Id");

                    b.ToTable("package");
                });

            modelBuilder.Entity("MaaDownloadServer.Model.Entities.PublicContent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<DateTime>("AddTime")
                        .HasColumnType("TEXT")
                        .HasColumnName("add_time");

                    b.Property<DateTime>("Duration")
                        .HasColumnType("TEXT")
                        .HasColumnName("duration");

                    b.Property<string>("FileExtension")
                        .HasColumnType("TEXT")
                        .HasColumnName("file_extension");

                    b.Property<string>("Hash")
                        .HasColumnType("TEXT")
                        .HasColumnName("hash");

                    b.Property<string>("Tag")
                        .HasColumnType("TEXT")
                        .HasColumnName("tag");

                    b.HasKey("Id");

                    b.ToTable("public_content");
                });

            modelBuilder.Entity("MaaDownloadServer.Model.Entities.Resource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("FileName")
                        .HasColumnType("TEXT")
                        .HasColumnName("file_name");

                    b.Property<string>("Hash")
                        .HasColumnType("TEXT")
                        .HasColumnName("hash");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT")
                        .HasColumnName("path");

                    b.HasKey("Id");

                    b.ToTable("resource");
                });

            modelBuilder.Entity("PackageResource", b =>
                {
                    b.Property<Guid>("PackagesId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ResourcesId")
                        .HasColumnType("TEXT");

                    b.HasKey("PackagesId", "ResourcesId");

                    b.HasIndex("ResourcesId");

                    b.ToTable("PackageResource");
                });

            modelBuilder.Entity("PackageResource", b =>
                {
                    b.HasOne("MaaDownloadServer.Model.Entities.Package", null)
                        .WithMany()
                        .HasForeignKey("PackagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MaaDownloadServer.Model.Entities.Resource", null)
                        .WithMany()
                        .HasForeignKey("ResourcesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}