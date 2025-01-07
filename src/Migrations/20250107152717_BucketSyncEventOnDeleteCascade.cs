using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class BucketSyncEventOnDeleteCascade : Migration
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BucketSyncEvents_Buckets_BucketId",
                table: "BucketSyncEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_BucketSyncEvents_Users_UserId",
                table: "BucketSyncEvents");

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
    }
}
