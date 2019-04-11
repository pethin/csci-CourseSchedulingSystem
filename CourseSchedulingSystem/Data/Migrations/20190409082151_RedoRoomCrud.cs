using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class RedoRoomCrud : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_RoomCapability_CapabilityId",
                table: "Room");

            migrationBuilder.RenameColumn(
                name: "CapabilityId",
                table: "Room",
                newName: "RoomCapabilityId");

            migrationBuilder.RenameIndex(
                name: "IX_Room_CapabilityId",
                table: "Room",
                newName: "IX_Room_RoomCapabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_RoomCapability_RoomCapabilityId",
                table: "Room",
                column: "RoomCapabilityId",
                principalTable: "RoomCapability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_RoomCapability_RoomCapabilityId",
                table: "Room");

            migrationBuilder.RenameColumn(
                name: "RoomCapabilityId",
                table: "Room",
                newName: "CapabilityId");

            migrationBuilder.RenameIndex(
                name: "IX_Room_RoomCapabilityId",
                table: "Room",
                newName: "IX_Room_CapabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_RoomCapability_CapabilityId",
                table: "Room",
                column: "CapabilityId",
                principalTable: "RoomCapability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
