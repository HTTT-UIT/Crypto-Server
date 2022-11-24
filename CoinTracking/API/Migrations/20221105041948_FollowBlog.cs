using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    public partial class FollowBlog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogEntityUserEntity",
                columns: table => new
                {
                    FollowBlogsId = table.Column<int>(type: "int", nullable: false),
                    FollowUsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogEntityUserEntity", x => new { x.FollowBlogsId, x.FollowUsersId });
                    table.ForeignKey(
                        name: "FK_BlogEntityUserEntity_Blogs_FollowBlogsId",
                        column: x => x.FollowBlogsId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogEntityUserEntity_Users_FollowUsersId",
                        column: x => x.FollowUsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogEntityUserEntity_FollowUsersId",
                table: "BlogEntityUserEntity",
                column: "FollowUsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogEntityUserEntity");
        }
    }
}
