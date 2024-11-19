using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdInOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderDetail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "OrderDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");
        }
    }
}
