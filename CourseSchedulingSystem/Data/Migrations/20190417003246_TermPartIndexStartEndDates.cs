using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class TermPartIndexStartEndDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TermParts_EndDate",
                table: "TermParts",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_TermParts_StartDate",
                table: "TermParts",
                column: "StartDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TermParts_EndDate",
                table: "TermParts");

            migrationBuilder.DropIndex(
                name: "IX_TermParts_StartDate",
                table: "TermParts");
        }
    }
}
