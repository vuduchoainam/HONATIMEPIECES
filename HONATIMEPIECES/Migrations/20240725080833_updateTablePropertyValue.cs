using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HONATIMEPIECES.Migrations
{
    /// <inheritdoc />
    public partial class updateTablePropertyValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "PropertyValues",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PropertyValues",
                newName: "Value");
        }
    }
}
