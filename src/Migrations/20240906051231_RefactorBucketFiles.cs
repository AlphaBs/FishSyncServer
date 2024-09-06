using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class RefactorBucketFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BucketFiles");

            migrationBuilder.CreateTable(
                name: "ChecksumStorageBucketFiles",
                columns: table => new
                {
                    BucketId = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Metadata_Checksum = table.Column<string>(type: "TEXT", nullable: false),
                    Metadata_LastUpdated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Metadata_Size = table.Column<long>(type: "INTEGER", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecksumStorageBucketFiles");

            migrationBuilder.CreateTable(
                name: "BucketFiles",
                columns: table => new
                {
                    BucketId = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata_Checksum = table.Column<string>(type: "TEXT", nullable: false),
                    Metadata_LastUpdated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Metadata_Size = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BucketFiles", x => new { x.BucketId, x.Path });
                    table.ForeignKey(
                        name: "FK_BucketFiles_Buckets_BucketId",
                        column: x => x.BucketId,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
