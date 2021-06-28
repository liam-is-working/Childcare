using Microsoft.EntityFrameworkCore.Migrations;

namespace Childcare.Migrations
{
    public partial class fixRela2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrators_AspNetUsers_ChildcareUserId",
                table: "Administrators");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AspNetUsers",
                newName: "ChildCareUserId");

            migrationBuilder.AlterColumn<string>(
                name: "ChildcareUserId",
                table: "Administrators",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AspNetUsersId",
                table: "Administrators",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Administrators_AspNetUsers_ChildcareUserId",
                table: "Administrators",
                column: "ChildcareUserId",
                principalTable: "AspNetUsers",
                principalColumn: "ChildCareUserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrators_AspNetUsers_ChildcareUserId",
                table: "Administrators");

            migrationBuilder.RenameColumn(
                name: "ChildCareUserId",
                table: "AspNetUsers",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "ChildcareUserId",
                table: "Administrators",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "AspNetUsersId",
                table: "Administrators",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Administrators_AspNetUsers_ChildcareUserId",
                table: "Administrators",
                column: "ChildcareUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
