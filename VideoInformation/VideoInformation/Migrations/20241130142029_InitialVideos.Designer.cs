﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VideoInformation.DBContexts;

#nullable disable

namespace VideoInformation.Migrations
{
    [DbContext(typeof(VideoContext))]
    [Migration("20241130142029_InitialVideos")]
    partial class InitialVideos
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("VideoInformation.Models.Video", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("id"));

                    b.Property<long>("AuthorID")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(510)
                        .HasColumnType("character varying(510)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("character varying(120)");

                    b.Property<DateTime>("UploadTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("id");

                    b.ToTable("Video");
                });

            modelBuilder.Entity("VideoInformation.Models.VideoComment", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("id"));

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("UserID")
                        .HasColumnType("bigint");

                    b.Property<long?>("Videoid")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.HasIndex("Videoid");

                    b.ToTable("VideoComment");
                });

            modelBuilder.Entity("VideoInformation.Models.VideoComment", b =>
                {
                    b.HasOne("VideoInformation.Models.Video", null)
                        .WithMany("Comments")
                        .HasForeignKey("Videoid");
                });

            modelBuilder.Entity("VideoInformation.Models.Video", b =>
                {
                    b.Navigation("Comments");
                });
#pragma warning restore 612, 618
        }
    }
}
