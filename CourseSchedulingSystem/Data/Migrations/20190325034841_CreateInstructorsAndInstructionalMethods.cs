using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class CreateInstructorsAndInstructionalMethods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstructionalMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    NormalizedName = table.Column<string>(nullable: true),
                    IsRoomRequired = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructionalMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    NormalizedFirstName = table.Column<string>(nullable: true),
                    Middle = table.Column<string>(nullable: true),
                    NormalizedMiddle = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: false),
                    NormalizedLastName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstructionalMethods_NormalizedName",
                table: "InstructionalMethods",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_NormalizedFirstName_NormalizedMiddle_NormalizedLastName",
                table: "Instructors",
                columns: new[] { "NormalizedFirstName", "NormalizedMiddle", "NormalizedLastName" },
                unique: true,
                filter: "[NormalizedFirstName] IS NOT NULL AND [NormalizedMiddle] IS NOT NULL AND [NormalizedLastName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructionalMethods");

            migrationBuilder.DropTable(
                name: "Instructors");
        }
    }
}
