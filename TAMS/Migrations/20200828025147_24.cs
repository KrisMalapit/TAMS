using Microsoft.EntityFrameworkCore.Migrations;

namespace TAMS.Migrations
{
    public partial class _24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Temperature2",
                table: "AttendanceLogs",
                type: "decimal(18,1)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Temperature1",
                table: "AttendanceLogs",
                type: "decimal(18,1)",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Temperature2",
                table: "AttendanceLogs",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Temperature1",
                table: "AttendanceLogs",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,1)");
        }
    }
}
