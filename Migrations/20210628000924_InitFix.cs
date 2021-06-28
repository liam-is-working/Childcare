using Microsoft.EntityFrameworkCore.Migrations;

namespace Childcare.Migrations
{
    public partial class InitFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AspNetUserId",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "AspNetUserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AspNetUserId",
                table: "Administrators");

            migrationBuilder.AddColumn<string>(
                name: "AspNetUsersId",
                table: "Staffs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AspNetUsersId",
                table: "Managers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AspNetUsersId",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AspNetUsersId",
                table: "Administrators",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AspNetUsersId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "AspNetUsersId",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "AspNetUsersId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AspNetUsersId",
                table: "Administrators");

            migrationBuilder.AddColumn<string>(
                name: "AspNetUserId",
                table: "Managers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AspNetUserId",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AspNetUserId",
                table: "Administrators",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
