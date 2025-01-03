using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveResScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponsibilityScore",
                table: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResponsibilityScore",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
