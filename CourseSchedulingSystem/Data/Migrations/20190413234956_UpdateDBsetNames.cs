using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class UpdateDBsetNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_Building_BuildingId",
                table: "Room");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomRoomCapability_RoomCapability_RoomCapabilityId",
                table: "RoomRoomCapability");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomRoomCapability_Room_RoomId",
                table: "RoomRoomCapability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomRoomCapability",
                table: "RoomRoomCapability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomCapability",
                table: "RoomCapability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Room",
                table: "Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Building",
                table: "Building");

            migrationBuilder.RenameTable(
                name: "RoomRoomCapability",
                newName: "RoomRoomCapabilities");

            migrationBuilder.RenameTable(
                name: "RoomCapability",
                newName: "RoomCapabilities");

            migrationBuilder.RenameTable(
                name: "Room",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "Building",
                newName: "Buildings");

            migrationBuilder.RenameIndex(
                name: "IX_RoomRoomCapability_RoomCapabilityId",
                table: "RoomRoomCapabilities",
                newName: "IX_RoomRoomCapabilities_RoomCapabilityId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomCapability_NormalizedName",
                table: "RoomCapabilities",
                newName: "IX_RoomCapabilities_NormalizedName");

            migrationBuilder.RenameIndex(
                name: "IX_Room_BuildingId_Number",
                table: "Rooms",
                newName: "IX_Rooms_BuildingId_Number");

            migrationBuilder.RenameIndex(
                name: "IX_Building_NormalizedName",
                table: "Buildings",
                newName: "IX_Buildings_NormalizedName");

            migrationBuilder.RenameIndex(
                name: "IX_Building_Code",
                table: "Buildings",
                newName: "IX_Buildings_Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomRoomCapabilities",
                table: "RoomRoomCapabilities",
                columns: new[] { "RoomId", "RoomCapabilityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomCapabilities",
                table: "RoomCapabilities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomRoomCapabilities_RoomCapabilities_RoomCapabilityId",
                table: "RoomRoomCapabilities",
                column: "RoomCapabilityId",
                principalTable: "RoomCapabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomRoomCapabilities_Rooms_RoomId",
                table: "RoomRoomCapabilities",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomRoomCapabilities_RoomCapabilities_RoomCapabilityId",
                table: "RoomRoomCapabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomRoomCapabilities_Rooms_RoomId",
                table: "RoomRoomCapabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomRoomCapabilities",
                table: "RoomRoomCapabilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomCapabilities",
                table: "RoomCapabilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Room");

            migrationBuilder.RenameTable(
                name: "RoomRoomCapabilities",
                newName: "RoomRoomCapability");

            migrationBuilder.RenameTable(
                name: "RoomCapabilities",
                newName: "RoomCapability");

            migrationBuilder.RenameTable(
                name: "Buildings",
                newName: "Building");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_BuildingId_Number",
                table: "Room",
                newName: "IX_Room_BuildingId_Number");

            migrationBuilder.RenameIndex(
                name: "IX_RoomRoomCapabilities_RoomCapabilityId",
                table: "RoomRoomCapability",
                newName: "IX_RoomRoomCapability_RoomCapabilityId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomCapabilities_NormalizedName",
                table: "RoomCapability",
                newName: "IX_RoomCapability_NormalizedName");

            migrationBuilder.RenameIndex(
                name: "IX_Buildings_NormalizedName",
                table: "Building",
                newName: "IX_Building_NormalizedName");

            migrationBuilder.RenameIndex(
                name: "IX_Buildings_Code",
                table: "Building",
                newName: "IX_Building_Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Room",
                table: "Room",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomRoomCapability",
                table: "RoomRoomCapability",
                columns: new[] { "RoomId", "RoomCapabilityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomCapability",
                table: "RoomCapability",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Building",
                table: "Building",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Building_BuildingId",
                table: "Room",
                column: "BuildingId",
                principalTable: "Building",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomRoomCapability_RoomCapability_RoomCapabilityId",
                table: "RoomRoomCapability",
                column: "RoomCapabilityId",
                principalTable: "RoomCapability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomRoomCapability_Room_RoomId",
                table: "RoomRoomCapability",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
