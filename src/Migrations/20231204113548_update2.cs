using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecksumStorages_Buckets_BucketEntityId",
                table: "ChecksumStorages");

            migrationBuilder.DropIndex(
                name: "IX_ChecksumStorages_BucketEntityId",
                table: "ChecksumStorages");

            migrationBuilder.DropColumn(
                name: "BucketEntityId",
                table: "ChecksumStorages");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecksumStorages_Buckets_BucketId",
                table: "ChecksumStorages",
                column: "BucketId",
                principalTable: "Buckets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecksumStorages_Buckets_BucketId",
                table: "ChecksumStorages");

            migrationBuilder.AddColumn<string>(
                name: "BucketEntityId",
                table: "ChecksumStorages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecksumStorages_BucketEntityId",
                table: "ChecksumStorages",
                column: "BucketEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecksumStorages_Buckets_BucketEntityId",
                table: "ChecksumStorages",
                column: "BucketEntityId",
                principalTable: "Buckets",
                principalColumn: "Id");
        }
    }
}
