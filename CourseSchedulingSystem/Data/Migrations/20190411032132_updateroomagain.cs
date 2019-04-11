using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class updateroomagain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_RoomCapability_RoomCapabilityId",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Room_RoomCapabilityId",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "RoomCapabilityId",
                table: "Room");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoomCapabilityId",
                table: "Room",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Room_RoomCapabilityId",
                table: "Room",
                column: "RoomCapabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_RoomCapability_RoomCapabilityId",
                table: "Room",
                column: "RoomCapabilityId",
                principalTable: "RoomCapability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
