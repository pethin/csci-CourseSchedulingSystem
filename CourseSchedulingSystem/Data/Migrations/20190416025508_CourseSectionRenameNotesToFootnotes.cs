using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class CourseSectionRenameNotesToFootnotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "CourseSections",
                newName: "Footnotes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Footnotes",
                table: "CourseSections",
                newName: "Notes");
        }
    }
}
