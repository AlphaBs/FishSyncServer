using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseBucketIdLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ChecksumStorageBuckets",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "BucketId",
                table: "ChecksumStorageBucketFiles",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "BucketId",
                table: "BucketSyncEvents",
                type: "character varying(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Buckets",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "BucketsId",
                table: "BucketEntityUserEntity",
                type: "character varying(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)");

            migrationBuilder.AlterColumn<string>(
                name: "DependenciesId",
                table: "BucketEntityBucketEntity",
                type: "character varying(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)");

            migrationBuilder.AlterColumn<string>(
                name: "BucketEntityId",
                table: "BucketEntityBucketEntity",
                type: "character varying(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AlphabetMirrorBuckets",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ChecksumStorageBuckets",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "BucketId",
                table: "ChecksumStorageBucketFiles",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "BucketId",
                table: "BucketSyncEvents",
                type: "character varying(16)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Buckets",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "BucketsId",
                table: "BucketEntityUserEntity",
                type: "character varying(16)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)");

            migrationBuilder.AlterColumn<string>(
                name: "DependenciesId",
                table: "BucketEntityBucketEntity",
                type: "character varying(16)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)");

            migrationBuilder.AlterColumn<string>(
                name: "BucketEntityId",
                table: "BucketEntityBucketEntity",
                type: "character varying(16)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AlphabetMirrorBuckets",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);
        }
    }
}
