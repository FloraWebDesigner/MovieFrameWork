using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovie.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFKViewerColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Viewers_Viewercustomer_id",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "Viewercustomer_id",
                table: "Tickets",
                newName: "viewer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_Viewercustomer_id",
                table: "Tickets",
                newName: "IX_Tickets_viewer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Viewers_viewer_id",
                table: "Tickets",
                column: "viewer_id",
                principalTable: "Viewers",
                principalColumn: "viewer_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Viewers_viewer_id",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "viewer_id",
                table: "Tickets",
                newName: "Viewercustomer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_viewer_id",
                table: "Tickets",
                newName: "IX_Tickets_Viewercustomer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Viewers_Viewercustomer_id",
                table: "Tickets",
                column: "Viewercustomer_id",
                principalTable: "Viewers",
                principalColumn: "viewer_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
