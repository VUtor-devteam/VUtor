using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLibrary.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManyTopicsOneFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserItems_Topics_TopicId",
                table: "UserItems");

            migrationBuilder.DropIndex(
                name: "IX_UserItems_TopicId",
                table: "UserItems");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "UserItems");

            migrationBuilder.CreateTable(
                name: "TopicToFiles",
                columns: table => new
                {
                    TopicsId = table.Column<int>(type: "int", nullable: false),
                    UserFilesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicToFiles", x => new { x.TopicsId, x.UserFilesId });
                    table.ForeignKey(
                        name: "FK_TopicToFiles_Topics_TopicsId",
                        column: x => x.TopicsId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicToFiles_UserItems_UserFilesId",
                        column: x => x.UserFilesId,
                        principalTable: "UserItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopicToFiles_UserFilesId",
                table: "TopicToFiles",
                column: "UserFilesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopicToFiles");

            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "UserItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserItems_TopicId",
                table: "UserItems",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserItems_Topics_TopicId",
                table: "UserItems",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
