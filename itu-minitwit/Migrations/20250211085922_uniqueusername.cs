using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace itu_minitwit.Migrations
{
    /// <inheritdoc />
    public partial class uniqueusername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_user_username",
                table: "user",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_username",
                table: "user");
        }
    }
}
