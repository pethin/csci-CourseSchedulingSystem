using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Migrations
{
    public partial class AddIdentifierIndexToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "LevelIndex",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_SubjectId",
                table: "Courses");

            migrationBuilder.CreateIndex(
                name: "IdentifierIndex",
                table: "Courses",
                columns: new[] { "SubjectId", "Level" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IdentifierIndex",
                table: "Courses");

            migrationBuilder.CreateIndex(
                name: "LevelIndex",
                table: "Courses",
                column: "Level",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SubjectId",
                table: "Courses",
                column: "SubjectId");
        }
    }
}
