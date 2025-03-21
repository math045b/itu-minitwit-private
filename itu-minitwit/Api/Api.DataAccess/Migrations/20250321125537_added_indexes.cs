using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class added_indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_message_author_id",
                table: "message");

            migrationBuilder.CreateIndex(
                name: "IX_message_author_id_flagged_pub_date",
                table: "message",
                columns: new[] { "author_id", "flagged", "pub_date" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_follower_who_id_whom_id",
                table: "follower",
                columns: new[] { "who_id", "whom_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_message_author_id_flagged_pub_date",
                table: "message");

            migrationBuilder.DropIndex(
                name: "IX_follower_who_id_whom_id",
                table: "follower");

            migrationBuilder.CreateIndex(
                name: "IX_message_author_id",
                table: "message",
                column: "author_id");
        }
    }
}
