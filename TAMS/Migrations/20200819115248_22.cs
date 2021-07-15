using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TAMS.Migrations
{
    public partial class _22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Temperature1",
                table: "AttendanceLogs",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Temperature2",
                table: "AttendanceLogs",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AttendanceLogs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Temperature1",
                table: "AttendanceLogs");

            migrationBuilder.DropColumn(
                name: "Temperature2",
                table: "AttendanceLogs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AttendanceLogs");
        }
    }
}
