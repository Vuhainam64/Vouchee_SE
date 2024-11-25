using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDateInModal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Modal");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Modal");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "VoucherCode",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "VoucherCode",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "VoucherCode");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "VoucherCode");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "Modal",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "Modal",
                type: "date",
                nullable: true);
        }
    }
}
