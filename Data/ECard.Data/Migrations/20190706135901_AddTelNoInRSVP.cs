using Microsoft.EntityFrameworkCore.Migrations;

namespace ECard.Data.Migrations
{
    public partial class AddTelNoInRSVP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TelNo",
                table: "Rsvp",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelNo",
                table: "Rsvp");
        }
    }
}
