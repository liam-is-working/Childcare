using Microsoft.EntityFrameworkCore.Migrations;

namespace Childcare.Migrations
{
    public partial class addSpecIdtoReservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecialtyID",
                table: "Reservations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecialtyID",
                table: "Reservations");
        }
    }
}
