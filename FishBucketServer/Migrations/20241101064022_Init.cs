using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChecksumStorages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    IsReadonly = table.Column<bool>(type: "boolean", nullable: false),
                    AccessKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    SecretKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    BucketName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Prefix = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ServiceEndpoint = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PublicEndpoint = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Host = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ClientSecret = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecksumStorages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Value = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    HashedPassword = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Roles = table.Column<string[]>(type: "text[]", nullable: false),
                    Email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Memo = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Buckets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    LastUpdated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ChecksumStorageId = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Limitations_ExpiredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Limitations_IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    Limitations_MaxBucketSize = table.Column<long>(type: "bigint", nullable: false),
                    Limitations_MaxFileSize = table.Column<long>(type: "bigint", nullable: false),
                    Limitations_MaxNumberOfFiles = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buckets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buckets_ChecksumStorages_ChecksumStorageId",
                        column: x => x.ChecksumStorageId,
                        principalTable: "ChecksumStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChecksumStorageBucketEntityUserEntity",
                columns: table => new
                {
                    BucketsId = table.Column<string>(type: "character varying(16)", nullable: false),
                    OwnersUsername = table.Column<string>(type: "character varying(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecksumStorageBucketEntityUserEntity", x => new { x.BucketsId, x.OwnersUsername });
                    table.ForeignKey(
                        name: "FK_ChecksumStorageBucketEntityUserEntity_Buckets_BucketsId",
                        column: x => x.BucketsId,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecksumStorageBucketEntityUserEntity_Users_OwnersUsername",
                        column: x => x.OwnersUsername,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecksumStorageBucketFiles",
                columns: table => new
                {
                    BucketId = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Path = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Metadata_Checksum = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Metadata_LastUpdated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Metadata_Size = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecksumStorageBucketFiles", x => new { x.BucketId, x.Path });
                    table.ForeignKey(
                        name: "FK_ChecksumStorageBucketFiles_Buckets_BucketId",
                        column: x => x.BucketId,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buckets_ChecksumStorageId",
                table: "Buckets",
                column: "ChecksumStorageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecksumStorageBucketEntityUserEntity_OwnersUsername",
                table: "ChecksumStorageBucketEntityUserEntity",
                column: "OwnersUsername");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecksumStorageBucketEntityUserEntity");

            migrationBuilder.DropTable(
                name: "ChecksumStorageBucketFiles");

            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Buckets");

            migrationBuilder.DropTable(
                name: "ChecksumStorages");
        }
    }
}
