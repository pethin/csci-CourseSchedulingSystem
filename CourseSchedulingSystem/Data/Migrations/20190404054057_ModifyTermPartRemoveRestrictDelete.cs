using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class ModifyTermPartRemoveRestrictDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TermParts_Terms_TermId",
                table: "TermParts");

            migrationBuilder.AddForeignKey(
                name: "FK_TermParts_Terms_TermId",
                table: "TermParts",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TermParts_Terms_TermId",
                table: "TermParts");

            migrationBuilder.AddForeignKey(
                name: "FK_TermParts_Terms_TermId",
                table: "TermParts",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
