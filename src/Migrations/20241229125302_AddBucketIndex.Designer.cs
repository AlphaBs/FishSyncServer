﻿// <auto-generated />
using System;
using System.Collections.Generic;
using AlphabetUpdateServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241229125302_AddBucketIndex")]
    partial class AddBucketIndex
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AlphabetUpdateServer.Entities.BucketEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<DateTimeOffset>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.ComplexProperty<Dictionary<string, object>>("Limitations", "AlphabetUpdateServer.Entities.BucketEntity.Limitations#BucketLimitations", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<DateTimeOffset>("ExpiredAt")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<bool>("IsReadOnly")
                                .HasColumnType("boolean");

                            b1.Property<long>("MaxBucketSize")
                                .HasColumnType("bigint");

                            b1.Property<long>("MaxFileSize")
                                .HasColumnType("bigint");

                            b1.Property<long>("MaxNumberOfFiles")
                                .HasColumnType("bigint");

                            b1.Property<int>("MonthlyMaxSyncCount")
                                .HasColumnType("integer");
                        });

                    b.HasKey("Id");

                    b.ToTable("Buckets");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.BucketIndexEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Description")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("Searchable")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("BucketIndexes");
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.BucketSyncEventEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BucketId")
                        .IsRequired()
                        .HasColumnType("character varying(64)");

                    b.Property<int>("EventType")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("character varying(16)");

                    b.HasKey("Id");

                    b.HasIndex("BucketId");

                    b.HasIndex("UserId");

                    b.ToTable("BucketSyncEvents");
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.ChecksumStorageBucketFileEntity", b =>
                {
                    b.Property<string>("BucketId")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Path")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.ComplexProperty<Dictionary<string, object>>("Metadata", "AlphabetUpdateServer.Entities.ChecksumStorageBucketFileEntity.Metadata#FileMetadata", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Checksum")
                                .IsRequired()
                                .HasMaxLength(64)
                                .HasColumnType("character varying(64)");

                            b1.Property<DateTimeOffset>("LastUpdated")
                                .HasColumnType("timestamp with time zone");

                            b1.Property<long>("Size")
                                .HasColumnType("bigint");
                        });

                    b.HasKey("BucketId", "Path");

                    b.ToTable("ChecksumStorageBucketFiles");
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.ChecksumStorageEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<bool>("IsReadonly")
                        .HasColumnType("boolean");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.HasKey("Id");

                    b.ToTable("ChecksumStorages");

                    b.HasDiscriminator<string>("Type").HasValue("base");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.ConfigEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.UserEntity", b =>
                {
                    b.Property<string>("Username")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<string>("Email")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Memo")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string[]>("Roles")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BucketEntityBucketEntity", b =>
                {
                    b.Property<string>("BucketEntityId")
                        .HasColumnType("character varying(64)");

                    b.Property<string>("DependenciesId")
                        .HasColumnType("character varying(64)");

                    b.HasKey("BucketEntityId", "DependenciesId");

                    b.HasIndex("DependenciesId");

                    b.ToTable("BucketEntityBucketEntity");
                });

            modelBuilder.Entity("BucketEntityBucketIndexEntity", b =>
                {
                    b.Property<string>("BucketIndexEntityId")
                        .HasColumnType("character varying(64)");

                    b.Property<string>("BucketsId")
                        .HasColumnType("character varying(64)");

                    b.HasKey("BucketIndexEntityId", "BucketsId");

                    b.HasIndex("BucketsId");

                    b.ToTable("BucketEntityBucketIndexEntity");
                });

            modelBuilder.Entity("BucketEntityUserEntity", b =>
                {
                    b.Property<string>("BucketsId")
                        .HasColumnType("character varying(64)");

                    b.Property<string>("OwnersUsername")
                        .HasColumnType("character varying(16)");

                    b.HasKey("BucketsId", "OwnersUsername");

                    b.HasIndex("OwnersUsername");

                    b.ToTable("BucketEntityUserEntity");
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.AlphabetMirrorBucketEntity", b =>
                {
                    b.HasBaseType("AlphabetUpdateServer.Entities.BucketEntity");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.ToTable("AlphabetMirrorBuckets");
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.ChecksumStorageBucketEntity", b =>
                {
                    b.HasBaseType("AlphabetUpdateServer.Entities.BucketEntity");

                    b.Property<string>("ChecksumStorageId")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.HasIndex("ChecksumStorageId");

                    b.ToTable("ChecksumStorageBuckets");
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.ObjectChecksumStorageEntity", b =>
                {
                    b.HasBaseType("AlphabetUpdateServer.Entities.ChecksumStorageEntity");

                    b.Property<string>("AccessKey")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("BucketName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("PublicEndpoint")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("SecretKey")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("ServiceEndpoint")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasDiscriminator().HasValue("object");
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.RFilesChecksumStorageEntity", b =>
                {
                    b.HasBaseType("AlphabetUpdateServer.Entities.ChecksumStorageEntity");

                    b.Property<string>("ClientSecret")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Host")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasDiscriminator().HasValue("rfiles");
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.BucketSyncEventEntity", b =>
                {
                    b.HasOne("AlphabetUpdateServer.Entities.BucketEntity", null)
                        .WithMany()
                        .HasForeignKey("BucketId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("AlphabetUpdateServer.Entities.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.ChecksumStorageBucketFileEntity", b =>
                {
                    b.HasOne("AlphabetUpdateServer.Entities.ChecksumStorageBucketEntity", null)
                        .WithMany("Files")
                        .HasForeignKey("BucketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BucketEntityBucketEntity", b =>
                {
                    b.HasOne("AlphabetUpdateServer.Entities.BucketEntity", null)
                        .WithMany()
                        .HasForeignKey("BucketEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlphabetUpdateServer.Entities.BucketEntity", null)
                        .WithMany()
                        .HasForeignKey("DependenciesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BucketEntityBucketIndexEntity", b =>
                {
                    b.HasOne("AlphabetUpdateServer.Entities.BucketIndexEntity", null)
                        .WithMany()
                        .HasForeignKey("BucketIndexEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlphabetUpdateServer.Entities.BucketEntity", null)
                        .WithMany()
                        .HasForeignKey("BucketsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BucketEntityUserEntity", b =>
                {
                    b.HasOne("AlphabetUpdateServer.Entities.BucketEntity", null)
                        .WithMany()
                        .HasForeignKey("BucketsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlphabetUpdateServer.Entities.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("OwnersUsername")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.AlphabetMirrorBucketEntity", b =>
                {
                    b.HasOne("AlphabetUpdateServer.Entities.BucketEntity", null)
                        .WithOne()
                        .HasForeignKey("AlphabetUpdateServer.Entities.AlphabetMirrorBucketEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.ChecksumStorageBucketEntity", b =>
                {
                    b.HasOne("AlphabetUpdateServer.Entities.ChecksumStorageEntity", null)
                        .WithMany()
                        .HasForeignKey("ChecksumStorageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AlphabetUpdateServer.Entities.BucketEntity", null)
                        .WithOne()
                        .HasForeignKey("AlphabetUpdateServer.Entities.ChecksumStorageBucketEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AlphabetUpdateServer.Entities.ChecksumStorageBucketEntity", b =>
                {
                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}