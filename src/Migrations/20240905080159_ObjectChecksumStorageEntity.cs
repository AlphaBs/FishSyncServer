using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class ObjectChecksumStorageEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessKey",
                table: "ChecksumStorages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BucketName",
                table: "ChecksumStorages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "ChecksumStorages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicEndpoint",
                table: "ChecksumStorages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretKey",
                table: "ChecksumStorages",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessKey",
                table: "ChecksumStorages");

            migrationBuilder.DropColumn(
                name: "BucketName",
                table: "ChecksumStorages");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "ChecksumStorages");

            migrationBuilder.DropColumn(
                name: "PublicEndpoint",
                table: "ChecksumStorages");

            migrationBuilder.DropColumn(
                name: "SecretKey",
                table: "ChecksumStorages");
        }
    }
}
