using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSomePropOfPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "MaxMoneyToDiscount",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "MinMoneyToApply",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "MoneyDiscount",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "RequiredQuantity",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Promotion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Promotion",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxMoneyToDiscount",
                table: "Promotion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinMoneyToApply",
                table: "Promotion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MoneyDiscount",
                table: "Promotion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredQuantity",
                table: "Promotion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Promotion",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
