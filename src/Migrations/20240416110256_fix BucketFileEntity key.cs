using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class fixBucketFileEntitykey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BucketFiles",
                table: "BucketFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "BucketFiles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BucketFiles",
                table: "BucketFiles",
                columns: new[] { "BucketId", "Path" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BucketFiles",
                table: "BucketFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "BucketFiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BucketFiles",
                table: "BucketFiles",
                columns: new[] { "BucketId", "Location" });
        }
    }
}
