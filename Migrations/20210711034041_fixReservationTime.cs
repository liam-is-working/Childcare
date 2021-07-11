using Microsoft.EntityFrameworkCore.Migrations;

namespace Childcare.Migrations
{
    public partial class fixReservationTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationTimes_Services_ServiceID",
                table: "ReservationTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationTimes",
                table: "ReservationTimes");

            migrationBuilder.DropIndex(
                name: "IX_ReservationTimes_ServiceID",
                table: "ReservationTimes");

            migrationBuilder.DropColumn(
                name: "ServiceID",
                table: "ReservationTimes");

            migrationBuilder.AddColumn<int>(
                name: "SpecialtyID",
                table: "ReservationTimes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationTimes",
                table: "ReservationTimes",
                columns: new[] { "Date", "SpecialtyID", "Slot" });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTimes_SpecialtyID",
                table: "ReservationTimes",
                column: "SpecialtyID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationTimes_Specialties_SpecialtyID",
                table: "ReservationTimes",
                column: "SpecialtyID",
                principalTable: "Specialties",
                principalColumn: "SpecialtyID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationTimes_Specialties_SpecialtyID",
                table: "ReservationTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationTimes",
                table: "ReservationTimes");

            migrationBuilder.DropIndex(
                name: "IX_ReservationTimes_SpecialtyID",
                table: "ReservationTimes");

            migrationBuilder.DropColumn(
                name: "SpecialtyID",
                table: "ReservationTimes");

            migrationBuilder.AddColumn<int>(
                name: "ServiceID",
                table: "ReservationTimes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationTimes",
                table: "ReservationTimes",
                columns: new[] { "Date", "ServiceID", "Slot" });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTimes_ServiceID",
                table: "ReservationTimes",
                column: "ServiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationTimes_Services_ServiceID",
                table: "ReservationTimes",
                column: "ServiceID",
                principalTable: "Services",
                principalColumn: "ServiceID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
