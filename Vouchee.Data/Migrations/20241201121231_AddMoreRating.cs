using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Star",
                table: "Rating",
                newName: "ServiceStar");

            migrationBuilder.AddColumn<int>(
                name: "QuantityStar",
                table: "Rating",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SellerStar",
                table: "Rating",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Rating",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityStar",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "SellerStar",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Rating");

            migrationBuilder.RenameColumn(
                name: "ServiceStar",
                table: "Rating",
                newName: "Star");
        }
    }
}
