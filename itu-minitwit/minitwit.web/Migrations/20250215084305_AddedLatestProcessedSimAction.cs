using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace itu_minitwit.Migrations
{
    /// <inheritdoc />
    public partial class AddedLatestProcessedSimAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LatestProcessedSimActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LatestProcessedSimActions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LatestProcessedSimActions");
        }
    }
}
