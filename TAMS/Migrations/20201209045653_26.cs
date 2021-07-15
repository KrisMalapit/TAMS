using Microsoft.EntityFrameworkCore.Migrations;

namespace TAMS.Migrations
{
    public partial class _26 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Clusters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Clusters_UserId",
                table: "Clusters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clusters_Users_UserId",
                table: "Clusters",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clusters_Users_UserId",
                table: "Clusters");

            migrationBuilder.DropIndex(
                name: "IX_Clusters_UserId",
                table: "Clusters");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Clusters");
        }
    }
}
