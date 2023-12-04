using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Checksums",
                table: "Checksums");

            migrationBuilder.RenameColumn(
                name: "Repository",
                table: "Checksums",
                newName: "Storage");

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "Checksums",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checksums",
                table: "Checksums",
                columns: new[] { "Location", "Checksum" });

            migrationBuilder.CreateTable(
                name: "ChecksumStorages",
                columns: table => new
                {
                    BucketId = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 34, nullable: false),
                    IsReadyOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    BucketEntityId = table.Column<string>(type: "TEXT", nullable: true),
                    ClientSecret = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecksumStorages", x => new { x.BucketId, x.Id });
                    table.ForeignKey(
                        name: "FK_ChecksumStorages_Buckets_BucketEntityId",
                        column: x => x.BucketEntityId,
                        principalTable: "Buckets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecksumStorages_BucketEntityId",
                table: "ChecksumStorages",
                column: "BucketEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecksumStorages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Checksums",
                table: "Checksums");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Checksums");

            migrationBuilder.RenameColumn(
                name: "Storage",
                table: "Checksums",
                newName: "Repository");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checksums",
                table: "Checksums",
                columns: new[] { "Checksum", "Repository" });
        }
    }
}
