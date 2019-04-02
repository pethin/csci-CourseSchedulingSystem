using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class AddOnDeleteRestrictConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAttributeTypes_AttributeTypes_AttributeTypeId",
                table: "CourseAttributeTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Subjects_SubjectId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseScheduleTypes_ScheduleTypes_ScheduleTypeId",
                table: "CourseScheduleTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUsers_Departments_DepartmentId",
                table: "DepartmentUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TermParts_Terms_TermId",
                table: "TermParts");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAttributeTypes_AttributeTypes_AttributeTypeId",
                table: "CourseAttributeTypes",
                column: "AttributeTypeId",
                principalTable: "AttributeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Subjects_SubjectId",
                table: "Courses",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseScheduleTypes_ScheduleTypes_ScheduleTypeId",
                table: "CourseScheduleTypes",
                column: "ScheduleTypeId",
                principalTable: "ScheduleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUsers_Departments_DepartmentId",
                table: "DepartmentUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TermParts_Terms_TermId",
                table: "TermParts",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAttributeTypes_AttributeTypes_AttributeTypeId",
                table: "CourseAttributeTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Subjects_SubjectId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseScheduleTypes_ScheduleTypes_ScheduleTypeId",
                table: "CourseScheduleTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentUsers_Departments_DepartmentId",
                table: "DepartmentUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TermParts_Terms_TermId",
                table: "TermParts");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAttributeTypes_AttributeTypes_AttributeTypeId",
                table: "CourseAttributeTypes",
                column: "AttributeTypeId",
                principalTable: "AttributeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Subjects_SubjectId",
                table: "Courses",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseScheduleTypes_ScheduleTypes_ScheduleTypeId",
                table: "CourseScheduleTypes",
                column: "ScheduleTypeId",
                principalTable: "ScheduleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentUsers_Departments_DepartmentId",
                table: "DepartmentUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TermParts_Terms_TermId",
                table: "TermParts",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
