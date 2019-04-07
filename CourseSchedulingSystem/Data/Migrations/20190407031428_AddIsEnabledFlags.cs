using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class AddIsEnabledFlags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUsers_Departments_DepartmentId",
                table: "DepartmentUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Terms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Courses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUsers_Departments_DepartmentId",
                table: "DepartmentUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUsers_Departments_DepartmentId",
                table: "DepartmentUsers");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Courses");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUsers_Departments_DepartmentId",
                table: "DepartmentUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
