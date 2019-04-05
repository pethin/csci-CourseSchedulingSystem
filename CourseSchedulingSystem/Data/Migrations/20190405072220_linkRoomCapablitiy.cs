using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class linkRoomCapablitiy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CapabilityId",
                table: "Room",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Room_CapabilityId",
                table: "Room",
                column: "CapabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Capability_CapabilityId",
                table: "Room",
                column: "CapabilityId",
                principalTable: "Capability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_Capability_CapabilityId",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Room_CapabilityId",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "CapabilityId",
                table: "Room");
        }
    }
}
