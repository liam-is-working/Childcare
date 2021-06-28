using Microsoft.EntityFrameworkCore.Migrations;

namespace Childcare.Migrations
{
    public partial class recre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrators_AspNetUsers_ChildcareUserId",
                table: "Administrators");

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
                name: "ChildcareUserId",
                table: "Staffs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChildcareUserId",
                table: "Managers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChildcareUserId",
                table: "Customers",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChildcareUserId",
                table: "Administrators",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_ChildcareUserId",
                table: "Staffs",
                column: "ChildcareUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_ChildcareUserId",
                table: "Managers",
                column: "ChildcareUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ChildcareUserId",
                table: "Customers",
                column: "ChildcareUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Administrators_AspNetUsers_ChildcareUserId",
                table: "Administrators",
                column: "ChildcareUserId",
                principalTable: "AspNetUsers",
                principalColumn: "ChildCareUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_ChildcareUserId",
                table: "Customers",
                column: "ChildcareUserId",
                principalTable: "AspNetUsers",
                principalColumn: "ChildCareUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_AspNetUsers_ChildcareUserId",
                table: "Managers",
                column: "ChildcareUserId",
                principalTable: "AspNetUsers",
                principalColumn: "ChildCareUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_AspNetUsers_ChildcareUserId",
                table: "Staffs",
                column: "ChildcareUserId",
                principalTable: "AspNetUsers",
                principalColumn: "ChildCareUserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Administrators_AspNetUsers_ChildcareUserId",
                table: "Administrators");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_ChildcareUserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Managers_AspNetUsers_ChildcareUserId",
                table: "Managers");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_AspNetUsers_ChildcareUserId",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_ChildcareUserId",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Managers_ChildcareUserId",
                table: "Managers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ChildcareUserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ChildcareUserId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "ChildcareUserId",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "ChildcareUserId",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "AspNetUsersId",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AspNetUsersId",
                table: "Managers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AspNetUsersId",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ChildcareUserId",
                table: "Administrators",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AspNetUsersId",
                table: "Administrators",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Administrators_AspNetUsers_ChildcareUserId",
                table: "Administrators",
                column: "ChildcareUserId",
                principalTable: "AspNetUsers",
                principalColumn: "ChildCareUserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
