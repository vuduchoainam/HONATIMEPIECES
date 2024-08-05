using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HONATIMEPIECES.Migrations
{
    /// <inheritdoc />
    public partial class updateTablePropertyProduct3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PropertyProducts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PropertyProducts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PropertyProducts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PropertyProducts",
                type: "datetime2",
                nullable: true);
        }
    }
}
