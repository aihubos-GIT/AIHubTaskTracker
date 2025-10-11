using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIHubTaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskToReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KPIs_Members_MemberId",
                table: "KPIs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KPIs",
                table: "KPIs");

            migrationBuilder.RenameTable(
                name: "KPIs",
                newName: "Kpis");

            migrationBuilder.RenameIndex(
                name: "IX_KPIs_MemberId",
                table: "Kpis",
                newName: "IX_Kpis_MemberId");

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Kpis",
                table: "Kpis",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_TaskId",
                table: "Reports",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kpis_Members_MemberId",
                table: "Kpis",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Tasks_TaskId",
                table: "Reports",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kpis_Members_MemberId",
                table: "Kpis");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Tasks_TaskId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_TaskId",
                table: "Reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Kpis",
                table: "Kpis");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Reports");

            migrationBuilder.RenameTable(
                name: "Kpis",
                newName: "KPIs");

            migrationBuilder.RenameIndex(
                name: "IX_Kpis_MemberId",
                table: "KPIs",
                newName: "IX_KPIs_MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KPIs",
                table: "KPIs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KPIs_Members_MemberId",
                table: "KPIs",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
