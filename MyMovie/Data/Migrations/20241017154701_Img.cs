using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovie.Data.Migrations
{
    /// <inheritdoc />
    public partial class Img : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPic",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PicExtension",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPic",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "PicExtension",
                table: "Movies");
        }
    }
}
