using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class RenameAttributeTypeToCourseAttribute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename CourseAttributeTypes to CourseCourseAttributes
            migrationBuilder.RenameTable(
                name: "CourseAttributeTypes",
                newName: "CourseCourseAttributes");

            migrationBuilder.RenameColumn(
                table: "CourseCourseAttributes",
                name: "AttributeTypeId",
                newName: "CourseAttributeId");

            // Rename AttributeTypes to CourseAttributes
            migrationBuilder.RenameTable(
                name: "AttributeTypes",
                newName: "CourseAttributes");

            // Drop pivot table foreign keys
            migrationBuilder.DropForeignKey(
                table: "CourseCourseAttributes",
                name: "FK_CourseAttributeTypes_AttributeTypes_AttributeTypeId");

            migrationBuilder.DropForeignKey(
                table: "CourseCourseAttributes",
                name: "FK_CourseAttributeTypes_Courses_CourseId");

            // Rename CourseAttributes PK
            migrationBuilder.DropPrimaryKey(
                table: "CourseAttributes",
                name: "PK_AttributeTypes");

            migrationBuilder.AddPrimaryKey(
                table: "CourseAttributes",
                name: "PK_CourseAttributes",
                column: "Id");

            // Rename CourseCourseAttributes PK
            migrationBuilder.DropPrimaryKey(
                table: "CourseCourseAttributes",
                name: "PK_CourseAttributeTypes");

            migrationBuilder.AddPrimaryKey(
                table: "CourseCourseAttributes",
                name: "PK_CourseCourseAttributes",
                columns: new[] {"CourseId", "CourseAttributeId"});

            // Recreate foreign keys
            migrationBuilder.AddForeignKey(
                table: "CourseCourseAttributes",
                column: "CourseAttributeId",
                name: "FK_CourseCourseAttributes_CourseAttributes_CourseAttributeId",
                principalTable: "CourseAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                table: "CourseCourseAttributes",
                column: "CourseId",
                name: "FK_CourseCourseAttributes_Courses_CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Rename indices
            migrationBuilder.RenameIndex(
                table: "CourseCourseAttributes",
                name: "IX_CourseAttributeTypes_AttributeTypeId",
                newName: "IX_CourseCourseAttributes_CourseAttributeId");

            migrationBuilder.RenameIndex(
                table: "CourseAttributes",
                name: "IX_AttributeTypes_NormalizedName",
                newName: "IX_CourseAttributes_NormalizedName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rename indices
            migrationBuilder.RenameIndex(
                table: "CourseCourseAttributes",
                newName: "IX_CourseAttributeTypes_AttributeTypeId",
                name: "IX_CourseCourseAttributes_CourseAttributeId");

            migrationBuilder.RenameIndex(
                table: "CourseAttributes",
                newName: "IX_AttributeTypes_NormalizedName",
                name: "IX_CourseAttributes_NormalizedName");

            // Drop pivot table foreign keys
            migrationBuilder.DropForeignKey(
                table: "CourseCourseAttributes",
                name: "FK_CourseCourseAttributes_CourseAttributes_CourseAttributeId");

            migrationBuilder.DropForeignKey(
                table: "CourseCourseAttributes",
                name: "FK_CourseCourseAttributes_Courses_CourseId");

            // Rename CourseAttributes PK
            migrationBuilder.DropPrimaryKey(
                table: "CourseAttributes",
                name: "PK_CourseAttributes");

            migrationBuilder.AddPrimaryKey(
                table: "CourseAttributes",
                name: "PK_AttributeTypes",
                column: "Id");

            // Rename CourseCourseAttributes PK
            migrationBuilder.DropPrimaryKey(
                table: "CourseCourseAttributes",
                name: "PK_CourseCourseAttributes");

            migrationBuilder.AddPrimaryKey(
                table: "CourseCourseAttributes",
                name: "PK_CourseAttributeTypes",
                columns: new[] {"CourseId", "CourseAttributeId"});

            // Recreate foreign keys
            migrationBuilder.AddForeignKey(
                table: "CourseCourseAttributes",
                column: "CourseAttributeId",
                name: "FK_CourseAttributeTypes_AttributeTypes_AttributeTypeId",
                principalTable: "CourseAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                table: "CourseCourseAttributes",
                column: "CourseId",
                name: "FK_CourseAttributeTypes_Courses_CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Rename AttributeTypes to CourseAttributes
            migrationBuilder.RenameTable(
                newName: "AttributeTypes",
                name: "CourseAttributes");

            // Rename CourseAttributeTypes to CourseCourseAttributes
            migrationBuilder.RenameTable(
                newName: "CourseAttributeTypes",
                name: "CourseCourseAttributes");

            migrationBuilder.RenameColumn(
                table: "CourseAttributeTypes",
                newName: "AttributeTypeId",
                name: "CourseAttributeId");
        }
    }
}