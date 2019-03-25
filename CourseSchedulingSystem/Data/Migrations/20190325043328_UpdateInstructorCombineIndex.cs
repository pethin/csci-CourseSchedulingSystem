using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class UpdateInstructorCombineIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Instructors_NormalizedFirstName_NormalizedMiddle_NormalizedLastName",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "NormalizedFirstName",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "NormalizedLastName",
                table: "Instructors");

            migrationBuilder.RenameColumn(
                name: "NormalizedMiddle",
                table: "Instructors",
                newName: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_NormalizedName",
                table: "Instructors",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Instructors_NormalizedName",
                table: "Instructors");

            migrationBuilder.RenameColumn(
                name: "NormalizedName",
                table: "Instructors",
                newName: "NormalizedMiddle");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedFirstName",
                table: "Instructors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedLastName",
                table: "Instructors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_NormalizedFirstName_NormalizedMiddle_NormalizedLastName",
                table: "Instructors",
                columns: new[] { "NormalizedFirstName", "NormalizedMiddle", "NormalizedLastName" },
                unique: true,
                filter: "[NormalizedFirstName] IS NOT NULL AND [NormalizedMiddle] IS NOT NULL AND [NormalizedLastName] IS NOT NULL");
        }
    }
}
