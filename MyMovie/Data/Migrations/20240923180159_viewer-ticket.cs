using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovie.Data.Migrations
{
    /// <inheritdoc />
    public partial class viewerticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Viewercustomer_id",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Viewercustomer_id",
                table: "Tickets",
                column: "Viewercustomer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Viewers_Viewercustomer_id",
                table: "Tickets",
                column: "Viewercustomer_id",
                principalTable: "Viewers",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Viewers_Viewercustomer_id",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_Viewercustomer_id",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Viewercustomer_id",
                table: "Tickets");
        }
    }
}
