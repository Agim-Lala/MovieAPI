using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class CleanupCommentReactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentDislikes");

            migrationBuilder.DropTable(
                name: "CommentLikes");

            migrationBuilder.DropColumn(
                name: "DislikesCount",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LikesCount",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DislikesCount",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikesCount",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CommentDislikes",
                columns: table => new
                {
                    DislikedByUsersId = table.Column<int>(type: "integer", nullable: false),
                    DislikedCommentsCommentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentDislikes", x => new { x.DislikedByUsersId, x.DislikedCommentsCommentId });
                    table.ForeignKey(
                        name: "FK_CommentDislikes_Comments_DislikedCommentsCommentId",
                        column: x => x.DislikedCommentsCommentId,
                        principalTable: "Comments",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentDislikes_Users_DislikedByUsersId",
                        column: x => x.DislikedByUsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentLikes",
                columns: table => new
                {
                    LikedByUsersId = table.Column<int>(type: "integer", nullable: false),
                    LikedCommentsCommentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentLikes", x => new { x.LikedByUsersId, x.LikedCommentsCommentId });
                    table.ForeignKey(
                        name: "FK_CommentLikes_Comments_LikedCommentsCommentId",
                        column: x => x.LikedCommentsCommentId,
                        principalTable: "Comments",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentLikes_Users_LikedByUsersId",
                        column: x => x.LikedByUsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentDislikes_DislikedCommentsCommentId",
                table: "CommentDislikes",
                column: "DislikedCommentsCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentLikes_LikedCommentsCommentId",
                table: "CommentLikes",
                column: "LikedCommentsCommentId");
        }
    }
}
