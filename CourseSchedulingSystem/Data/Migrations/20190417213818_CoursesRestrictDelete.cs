using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class CoursesRestrictDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSections_Courses_CourseId",
                table: "CourseSections");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSections_Courses_CourseId",
                table: "CourseSections",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSections_Courses_CourseId",
                table: "CourseSections");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSections_Courses_CourseId",
                table: "CourseSections",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
