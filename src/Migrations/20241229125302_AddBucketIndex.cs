using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class AddBucketIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BucketIndexes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Searchable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BucketIndexes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BucketEntityBucketIndexEntity",
                columns: table => new
                {
                    BucketIndexEntityId = table.Column<string>(type: "character varying(64)", nullable: false),
                    BucketsId = table.Column<string>(type: "character varying(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BucketEntityBucketIndexEntity", x => new { x.BucketIndexEntityId, x.BucketsId });
                    table.ForeignKey(
                        name: "FK_BucketEntityBucketIndexEntity_BucketIndexes_BucketIndexEnti~",
                        column: x => x.BucketIndexEntityId,
                        principalTable: "BucketIndexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BucketEntityBucketIndexEntity_Buckets_BucketsId",
                        column: x => x.BucketsId,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BucketEntityBucketIndexEntity_BucketsId",
                table: "BucketEntityBucketIndexEntity",
                column: "BucketsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BucketEntityBucketIndexEntity");

            migrationBuilder.DropTable(
                name: "BucketIndexes");
        }
    }
}
