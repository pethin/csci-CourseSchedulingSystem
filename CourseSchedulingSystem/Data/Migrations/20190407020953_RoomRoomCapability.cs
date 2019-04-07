using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseSchedulingSystem.Data.Migrations
{
    public partial class RoomRoomCapability : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_Capability_CapabilityId",
                table: "Room");

            migrationBuilder.DropTable(
                name: "Capability");

            migrationBuilder.CreateTable(
                name: "RoomCapability",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    NormalizedName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomCapability", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomRoomCapability",
                columns: table => new
                {
                    RoomCapabilityId = table.Column<Guid>(nullable: false),
                    RoomId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomRoomCapability", x => new { x.RoomId, x.RoomCapabilityId });
                    table.ForeignKey(
                        name: "FK_RoomRoomCapability_RoomCapability_RoomCapabilityId",
                        column: x => x.RoomCapabilityId,
                        principalTable: "RoomCapability",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomRoomCapability_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomCapability_NormalizedName",
                table: "RoomCapability",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoomRoomCapability_RoomCapabilityId",
                table: "RoomRoomCapability",
                column: "RoomCapabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_RoomCapability_CapabilityId",
                table: "Room",
                column: "CapabilityId",
                principalTable: "RoomCapability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_RoomCapability_CapabilityId",
                table: "Room");

            migrationBuilder.DropTable(
                name: "RoomRoomCapability");

            migrationBuilder.DropTable(
                name: "RoomCapability");

            migrationBuilder.CreateTable(
                name: "Capability",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    NormalizedName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capability", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Capability_NormalizedName",
                table: "Capability",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Capability_CapabilityId",
                table: "Room",
                column: "CapabilityId",
                principalTable: "Capability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
