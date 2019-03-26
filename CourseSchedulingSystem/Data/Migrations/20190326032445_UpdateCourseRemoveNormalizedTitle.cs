using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class UpdateCourseRemoveNormalizedTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_NormalizedTitle",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "NormalizedTitle",
                table: "Courses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedTitle",
                table: "Courses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_NormalizedTitle",
                table: "Courses",
                column: "NormalizedTitle",
                unique: true,
                filter: "[NormalizedTitle] IS NOT NULL");
        }
    }
}
