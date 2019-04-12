using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class unrestrictdelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomRoomCapability_RoomCapability_RoomCapabilityId",
                table: "RoomRoomCapability");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomRoomCapability_RoomCapability_RoomCapabilityId",
                table: "RoomRoomCapability",
                column: "RoomCapabilityId",
                principalTable: "RoomCapability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomRoomCapability_RoomCapability_RoomCapabilityId",
                table: "RoomRoomCapability");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomRoomCapability_RoomCapability_RoomCapabilityId",
                table: "RoomRoomCapability",
                column: "RoomCapabilityId",
                principalTable: "RoomCapability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
