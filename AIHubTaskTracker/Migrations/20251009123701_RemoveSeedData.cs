using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIHubTaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "CreatedAt", "Deadline", "MemberName", "Status", "TaskTitle" },
                values: new object[] { 1, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nguyễn Thành Tuấn", "In Progress", "Tích hợp API Dashboard" });
        }
    }
}
