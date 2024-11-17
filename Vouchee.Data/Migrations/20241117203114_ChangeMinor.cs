using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMinor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVerfied",
                table: "Brand",
                newName: "IsVerified");

            migrationBuilder.RenameColumn(
                name: "IsVerfied",
                table: "Address",
                newName: "IsVerified");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "VoucherCode",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "VerifiedBy",
                table: "VoucherCode",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedDate",
                table: "VoucherCode",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "VoucherCode");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "VoucherCode");

            migrationBuilder.DropColumn(
                name: "VerifiedDate",
                table: "VoucherCode");

            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "Brand",
                newName: "IsVerfied");

            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "Address",
                newName: "IsVerfied");
        }
    }
}
