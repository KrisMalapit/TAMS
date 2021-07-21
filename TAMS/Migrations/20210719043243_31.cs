using Microsoft.EntityFrameworkCore.Migrations;

namespace TAMS.Migrations
{
    public partial class _31 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClusterUsers_Clusters_ClusterId",
                table: "ClusterUsers");

            migrationBuilder.DropIndex(
                name: "IX_ClusterUsers_ClusterId",
                table: "ClusterUsers");

            migrationBuilder.RenameColumn(
                name: "ClusterId",
                table: "ClusterUsers",
                newName: "DepartmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "ClusterUsers",
                newName: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_ClusterUsers_ClusterId",
                table: "ClusterUsers",
                column: "ClusterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClusterUsers_Clusters_ClusterId",
                table: "ClusterUsers",
                column: "ClusterId",
                principalTable: "Clusters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
