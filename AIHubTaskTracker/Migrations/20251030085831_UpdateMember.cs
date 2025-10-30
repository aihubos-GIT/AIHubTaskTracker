using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIHubTaskTracker.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "avatar_url",
                table: "Members",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Members",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar_url",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Members");
        }
    }
}
