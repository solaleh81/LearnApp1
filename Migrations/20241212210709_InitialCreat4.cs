using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreat4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryUser_Category_CategoryId",
                table: "CategoryUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryUser_Users_UserId",
                table: "CategoryUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryUser",
                table: "CategoryUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "CategoryUser",
                newName: "CategoryUsers");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryUser_UserId",
                table: "CategoryUsers",
                newName: "IX_CategoryUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryUser_CategoryId",
                table: "CategoryUsers",
                newName: "IX_CategoryUsers_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryUsers",
                table: "CategoryUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryUsers_Categories_CategoryId",
                table: "CategoryUsers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryUsers_Users_UserId",
                table: "CategoryUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryUsers_Categories_CategoryId",
                table: "CategoryUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryUsers_Users_UserId",
                table: "CategoryUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryUsers",
                table: "CategoryUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "CategoryUsers",
                newName: "CategoryUser");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryUsers_UserId",
                table: "CategoryUser",
                newName: "IX_CategoryUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryUsers_CategoryId",
                table: "CategoryUser",
                newName: "IX_CategoryUser_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryUser",
                table: "CategoryUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryUser_Category_CategoryId",
                table: "CategoryUser",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryUser_Users_UserId",
                table: "CategoryUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
