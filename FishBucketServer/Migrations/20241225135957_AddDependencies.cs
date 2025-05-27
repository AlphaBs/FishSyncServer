using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class AddDependencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BucketEntityBucketEntity",
                columns: table => new
                {
                    BucketEntityId = table.Column<string>(type: "character varying(16)", nullable: false),
                    DependenciesId = table.Column<string>(type: "character varying(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BucketEntityBucketEntity", x => new { x.BucketEntityId, x.DependenciesId });
                    table.ForeignKey(
                        name: "FK_BucketEntityBucketEntity_Buckets_BucketEntityId",
                        column: x => x.BucketEntityId,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BucketEntityBucketEntity_Buckets_DependenciesId",
                        column: x => x.DependenciesId,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BucketEntityBucketEntity_DependenciesId",
                table: "BucketEntityBucketEntity",
                column: "DependenciesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BucketEntityBucketEntity");
        }
    }
}
