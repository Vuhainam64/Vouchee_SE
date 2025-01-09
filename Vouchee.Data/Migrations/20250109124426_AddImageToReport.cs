using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageToReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReportId",
                table: "Media",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Media_ReportId",
                table: "Media",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Report_ReportId",
                table: "Media",
                column: "ReportId",
                principalTable: "Report",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Report_ReportId",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Media_ReportId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "Media");
        }
    }
}
