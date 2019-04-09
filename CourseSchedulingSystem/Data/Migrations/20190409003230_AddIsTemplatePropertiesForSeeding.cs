using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class AddIsTemplatePropertiesForSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_Building_BuildingId",
                table: "Room");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledMeetingTimeRooms_Room_RoomId",
                table: "ScheduledMeetingTimeRooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Room",
                table: "Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Building",
                table: "Building");

            migrationBuilder.RenameTable(
                name: "Room",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "Building",
                newName: "Buildings");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsTemplate",
                table: "Subjects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemplate",
                table: "ScheduleTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemplate",
                table: "MeetingTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemplate",
                table: "InstructionalMethods",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemplate",
                table: "Departments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemplate",
                table: "CourseAttributes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemplate",
                table: "Buildings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledMeetingTimeRooms_Rooms_RoomId",
                table: "ScheduledMeetingTimeRooms",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledMeetingTimeRooms_Rooms_RoomId",
                table: "ScheduledMeetingTimeRooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Buildings",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "IsTemplate",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "IsTemplate",
                table: "ScheduleTypes");

            migrationBuilder.DropColumn(
                name: "IsTemplate",
                table: "MeetingTypes");

            migrationBuilder.DropColumn(
                name: "IsTemplate",
                table: "InstructionalMethods");

            migrationBuilder.DropColumn(
                name: "IsTemplate",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "IsTemplate",
                table: "CourseAttributes");

            migrationBuilder.DropColumn(
                name: "IsTemplate",
                table: "Buildings");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Room");

            migrationBuilder.RenameTable(
                name: "Buildings",
                newName: "Building");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_BuildingId_Number",
                table: "Room",
                newName: "IX_Room_BuildingId_Number");

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
                name: "FK_ScheduledMeetingTimeRooms_Room_RoomId",
                table: "ScheduledMeetingTimeRooms",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
