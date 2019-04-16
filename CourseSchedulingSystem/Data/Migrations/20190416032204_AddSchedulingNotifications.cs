using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class AddSchedulingNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SchedulingNotifications",
                table: "ScheduledMeetingTimes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchedulingNotifications",
                table: "CourseSections",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SchedulingNotifications",
                table: "ScheduledMeetingTimes");

            migrationBuilder.DropColumn(
                name: "SchedulingNotifications",
                table: "CourseSections");
        }
    }
}
