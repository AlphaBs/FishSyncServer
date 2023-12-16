using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphabetUpdateServer.Migrations
{
    /// <inheritdoc />
    public partial class User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BucketEntityUserEntity");

            migrationBuilder.DropTable(
                name: "UserEntity");

            migrationBuilder.AddColumn<string>(
                name: "BucketEntityId",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discord",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BucketEntityId",
                table: "AspNetUsers",
                column: "BucketEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Buckets_BucketEntityId",
                table: "AspNetUsers",
                column: "BucketEntityId",
                principalTable: "Buckets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Buckets_BucketEntityId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BucketEntityId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BucketEntityId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discord",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "UserEntity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BucketEntityUserEntity",
                columns: table => new
                {
                    BucketsId = table.Column<string>(type: "TEXT", nullable: false),
                    OwnersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BucketEntityUserEntity", x => new { x.BucketsId, x.OwnersId });
                    table.ForeignKey(
                        name: "FK_BucketEntityUserEntity_Buckets_BucketsId",
                        column: x => x.BucketsId,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BucketEntityUserEntity_UserEntity_OwnersId",
                        column: x => x.OwnersId,
                        principalTable: "UserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BucketEntityUserEntity_OwnersId",
                table: "BucketEntityUserEntity",
                column: "OwnersId");
        }
    }
}
