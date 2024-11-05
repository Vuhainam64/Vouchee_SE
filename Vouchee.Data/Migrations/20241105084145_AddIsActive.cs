using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Voucher",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Modal",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Modal");
        }
    }
}
