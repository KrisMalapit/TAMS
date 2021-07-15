using Microsoft.EntityFrameworkCore.Migrations;

namespace TAMS.Migrations
{
    public partial class update12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LevelId",
                table: "Employees",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Levels_LevelId",
                table: "Employees",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Levels_LevelId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_LevelId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "Employees");
        }
    }
}
