using Microsoft.EntityFrameworkCore.Migrations;

namespace TAMS.Migrations
{
    public partial class update17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttendanceLogs_EmployeeId_CreatedDate_Status",
                table: "AttendanceLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AttendanceLogs",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLogs_EmployeeId",
                table: "AttendanceLogs",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttendanceLogs_EmployeeId",
                table: "AttendanceLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AttendanceLogs",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLogs_EmployeeId_CreatedDate_Status",
                table: "AttendanceLogs",
                columns: new[] { "EmployeeId", "CreatedDate", "Status" },
                unique: true,
                filter: "[Status] IS NOT NULL");
        }
    }
}
