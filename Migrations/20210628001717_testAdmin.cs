using Microsoft.EntityFrameworkCore.Migrations;

namespace Childcare.Migrations
{
    public partial class testAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChildcareUserId",
                table: "Administrators",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_ChildcareUserId",
                table: "Administrators",
                column: "ChildcareUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Administrators_AspNetUsers_ChildcareUserId",
                table: "Administrators",
                column: "ChildcareUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrators_AspNetUsers_ChildcareUserId",
                table: "Administrators");

            migrationBuilder.DropIndex(
                name: "IX_Administrators_ChildcareUserId",
                table: "Administrators");

            migrationBuilder.DropColumn(
                name: "ChildcareUserId",
                table: "Administrators");
        }
    }
}
