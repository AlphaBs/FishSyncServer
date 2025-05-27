using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class UseTPTForBuckets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlphabetMirrorBuckets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlphabetMirrorBuckets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlphabetMirrorBuckets_Buckets_Id",
                        column: x => x.Id,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecksumStorageBuckets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ChecksumStorageId = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecksumStorageBuckets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecksumStorageBuckets_Buckets_Id",
                        column: x => x.Id,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecksumStorageBuckets_ChecksumStorages_ChecksumStorageId",
                        column: x => x.ChecksumStorageId,
                        principalTable: "ChecksumStorages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecksumStorageBuckets_ChecksumStorageId",
                table: "ChecksumStorageBuckets",
                column: "ChecksumStorageId");

            migrationBuilder.Sql(@"
                INSERT INTO ""ChecksumStorageBuckets"" (""Id"", ""ChecksumStorageId"")
                SELECT ""Id"", ""ChecksumStorageId""
                From ""Buckets""
                WHERE ""ChecksumStorageId"" IS NOT NULL;");
            
            migrationBuilder.AddForeignKey(
                name: "FK_ChecksumStorageBucketFiles_ChecksumStorageBuckets_BucketId",
                table: "ChecksumStorageBucketFiles",
                column: "BucketId",
                principalTable: "ChecksumStorageBuckets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            
            migrationBuilder.DropForeignKey(
                name: "FK_Buckets_ChecksumStorages_ChecksumStorageId",
                table: "Buckets");

            migrationBuilder.DropForeignKey(
                name: "FK_ChecksumStorageBucketFiles_Buckets_BucketId",
                table: "ChecksumStorageBucketFiles");

            migrationBuilder.DropIndex(
                name: "IX_Buckets_ChecksumStorageId",
                table: "Buckets");

            migrationBuilder.DropColumn(
                name: "ChecksumStorageId",
                table: "Buckets");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Buckets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChecksumStorageId",
                table: "Buckets",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Buckets",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
            
            migrationBuilder.CreateIndex(
                name: "IX_Buckets_ChecksumStorageId",
                table: "Buckets",
                column: "ChecksumStorageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buckets_ChecksumStorages_ChecksumStorageId",
                table: "Buckets",
                column: "ChecksumStorageId",
                principalTable: "ChecksumStorages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecksumStorageBucketFiles_Buckets_BucketId",
                table: "ChecksumStorageBucketFiles",
                column: "BucketId",
                principalTable: "Buckets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            
            migrationBuilder.DropForeignKey(
                name: "FK_ChecksumStorageBucketFiles_ChecksumStorageBuckets_BucketId",
                table: "ChecksumStorageBucketFiles");

            migrationBuilder.DropTable(
                name: "AlphabetMirrorBuckets");

            migrationBuilder.DropTable(
                name: "ChecksumStorageBuckets");
        }
    }
}
