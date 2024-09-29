using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovie.Data.Migrations
{
    /// <inheritdoc />
    public partial class movieticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "movie_id",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_movie_id",
                table: "Tickets",
                column: "movie_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Movies_movie_id",
                table: "Tickets",
                column: "movie_id",
                principalTable: "Movies",
                principalColumn: "movie_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Movies_movie_id",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_movie_id",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "movie_id",
                table: "Tickets");
        }
    }
}
