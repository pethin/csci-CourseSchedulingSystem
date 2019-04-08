using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class CreateCourseSectionSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TermPartId = table.Column<Guid>(nullable: false),
                    CourseId = table.Column<Guid>(nullable: false),
                    Section = table.Column<int>(nullable: false),
                    ScheduleTypeId = table.Column<Guid>(nullable: false),
                    InstructionalMethodId = table.Column<Guid>(nullable: false),
                    MaximumCapacity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseSections_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseSections_InstructionalMethods_InstructionalMethodId",
                        column: x => x.InstructionalMethodId,
                        principalTable: "InstructionalMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseSections_ScheduleTypes_ScheduleTypeId",
                        column: x => x.ScheduleTypeId,
                        principalTable: "ScheduleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseSections_TermParts_TermPartId",
                        column: x => x.TermPartId,
                        principalTable: "TermParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeetingTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    NormalizedName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledMeetingTimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CourseSectionId = table.Column<Guid>(nullable: false),
                    MeetingTypeId = table.Column<Guid>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    Monday = table.Column<bool>(nullable: false),
                    Tuesday = table.Column<bool>(nullable: false),
                    Wednesday = table.Column<bool>(nullable: false),
                    Thursday = table.Column<bool>(nullable: false),
                    Friday = table.Column<bool>(nullable: false),
                    Saturday = table.Column<bool>(nullable: false),
                    Sunday = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledMeetingTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledMeetingTimes_CourseSections_CourseSectionId",
                        column: x => x.CourseSectionId,
                        principalTable: "CourseSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledMeetingTimes_MeetingTypes_MeetingTypeId",
                        column: x => x.MeetingTypeId,
                        principalTable: "MeetingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledMeetingTimeInstructors",
                columns: table => new
                {
                    ScheduledMeetingTimeId = table.Column<Guid>(nullable: false),
                    InstructorId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledMeetingTimeInstructors", x => new { x.ScheduledMeetingTimeId, x.InstructorId });
                    table.ForeignKey(
                        name: "FK_ScheduledMeetingTimeInstructors_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduledMeetingTimeInstructors_ScheduledMeetingTimes_ScheduledMeetingTimeId",
                        column: x => x.ScheduledMeetingTimeId,
                        principalTable: "ScheduledMeetingTimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledMeetingTimeRooms",
                columns: table => new
                {
                    ScheduledMeetingTimeId = table.Column<Guid>(nullable: false),
                    RoomId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledMeetingTimeRooms", x => new { x.ScheduledMeetingTimeId, x.RoomId });
                    table.ForeignKey(
                        name: "FK_ScheduledMeetingTimeRooms_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduledMeetingTimeRooms_ScheduledMeetingTimes_ScheduledMeetingTimeId",
                        column: x => x.ScheduledMeetingTimeId,
                        principalTable: "ScheduledMeetingTimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseSections_CourseId",
                table: "CourseSections",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSections_InstructionalMethodId",
                table: "CourseSections",
                column: "InstructionalMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSections_ScheduleTypeId",
                table: "CourseSections",
                column: "ScheduleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSections_TermPartId_CourseId_Section",
                table: "CourseSections",
                columns: new[] { "TermPartId", "CourseId", "Section" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeetingTypes_Code",
                table: "MeetingTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeetingTypes_NormalizedName",
                table: "MeetingTypes",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMeetingTimeInstructors_InstructorId",
                table: "ScheduledMeetingTimeInstructors",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMeetingTimeRooms_RoomId",
                table: "ScheduledMeetingTimeRooms",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMeetingTimes_CourseSectionId",
                table: "ScheduledMeetingTimes",
                column: "CourseSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMeetingTimes_MeetingTypeId",
                table: "ScheduledMeetingTimes",
                column: "MeetingTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledMeetingTimeInstructors");

            migrationBuilder.DropTable(
                name: "ScheduledMeetingTimeRooms");

            migrationBuilder.DropTable(
                name: "ScheduledMeetingTimes");

            migrationBuilder.DropTable(
                name: "CourseSections");

            migrationBuilder.DropTable(
                name: "MeetingTypes");
        }
    }
}
