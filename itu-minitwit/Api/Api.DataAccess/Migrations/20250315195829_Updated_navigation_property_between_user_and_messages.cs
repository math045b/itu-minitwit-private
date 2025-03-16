using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Updated_navigation_property_between_user_and_messages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_message_author_id",
                table: "message",
                column: "author_id");

            migrationBuilder.AddForeignKey(
                name: "FK_message_user_author_id",
                table: "message",
                column: "author_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_message_user_author_id",
                table: "message");

            migrationBuilder.DropIndex(
                name: "IX_message_author_id",
                table: "message");
        }
    }
}
