using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HONATIMEPIECES.Migrations
{
    /// <inheritdoc />
    public partial class updateTablePropertyProduct5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyProducts",
                table: "PropertyProducts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyProducts",
                table: "PropertyProducts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyProducts_ProductId",
                table: "PropertyProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyProducts",
                table: "PropertyProducts");

            migrationBuilder.DropIndex(
                name: "IX_PropertyProducts_ProductId",
                table: "PropertyProducts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyProducts",
                table: "PropertyProducts",
                columns: new[] { "ProductId", "PropertyId", "PropertyValueId" });
        }
    }
}
