using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class AddCodesToScheduleTypeInstructionalMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ScheduleTypes",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "InstructionalMethods",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTypes_Code",
                table: "ScheduleTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstructionalMethods_Code",
                table: "InstructionalMethods",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduleTypes_Code",
                table: "ScheduleTypes");

            migrationBuilder.DropIndex(
                name: "IX_InstructionalMethods_Code",
                table: "InstructionalMethods");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ScheduleTypes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "InstructionalMethods");
        }
    }
}
