using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class DivideBucketEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BucketSyncEvents_Buckets_BucketId",
                table: "BucketSyncEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_BucketSyncEvents_Users_UserId",
                table: "BucketSyncEvents");

            migrationBuilder.AlterColumn<string>(
                name: "ChecksumStorageId",
                table: "Buckets",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Buckets",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "checksum-storage");

            migrationBuilder.CreateTable(
                name: "BucketEntityUserEntity",
                columns: table => new
                {
                    BucketsId = table.Column<string>(type: "character varying(16)", nullable: false),
                    OwnersUsername = table.Column<string>(type: "character varying(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BucketEntityUserEntity", x => new { x.BucketsId, x.OwnersUsername });
                    table.ForeignKey(
                        name: "FK_BucketEntityUserEntity_Buckets_BucketsId",
                        column: x => x.BucketsId,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BucketEntityUserEntity_Users_OwnersUsername",
                        column: x => x.OwnersUsername,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BucketEntityUserEntity_OwnersUsername",
                table: "BucketEntityUserEntity",
                column: "OwnersUsername");

            migrationBuilder.Sql(@"
                INSERT INTO ""BucketEntityUserEntity"" (""BucketsId"", ""OwnersUsername"")
                SELECT ""BucketsId"", ""OwnersUsername"" 
                FROM ""ChecksumStorageBucketEntityUserEntity""
            ");
            
            migrationBuilder.DropTable(
                name: "ChecksumStorageBucketEntityUserEntity");

            migrationBuilder.AddForeignKey(
                name: "FK_BucketSyncEvents_Buckets_BucketId",
                table: "BucketSyncEvents",
                column: "BucketId",
                principalTable: "Buckets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BucketSyncEvents_Users_UserId",
                table: "BucketSyncEvents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BucketSyncEvents_Buckets_BucketId",
                table: "BucketSyncEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_BucketSyncEvents_Users_UserId",
                table: "BucketSyncEvents");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Buckets");

            migrationBuilder.AlterColumn<string>(
                name: "ChecksumStorageId",
                table: "Buckets",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_ChecksumStorageBucketEntityUserEntity_OwnersUsername",
                table: "ChecksumStorageBucketEntityUserEntity",
                column: "OwnersUsername");

            migrationBuilder.Sql(@"
                INSERT INTO ""ChecksumStorageBucketEntityUserEntity"" (""BucketsId"", ""OwnersUsername"")
                SELECT ""BucketsId"", ""OwnersUsername"" 
                FROM ""BucketEntityUserEntity""
            ");
            
            migrationBuilder.DropTable(
                name: "BucketEntityUserEntity");
            
            migrationBuilder.AddForeignKey(
                name: "FK_BucketSyncEvents_Buckets_BucketId",
                table: "BucketSyncEvents",
                column: "BucketId",
                principalTable: "Buckets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BucketSyncEvents_Users_UserId",
                table: "BucketSyncEvents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
