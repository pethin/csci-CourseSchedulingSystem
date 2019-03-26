using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class UpdateCoursesAddCourseLevels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Courses",
                newName: "Number");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_SubjectId_Level",
                table: "Courses",
                newName: "IX_Courses_SubjectId_Number");

            migrationBuilder.AddColumn<bool>(
                name: "IsGraduate",
                table: "Courses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUndergraduate",
                table: "Courses",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGraduate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "IsUndergraduate",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Courses",
                newName: "Level");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_SubjectId_Number",
                table: "Courses",
                newName: "IX_Courses_SubjectId_Level");
        }
    }
}
