using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HONATIMEPIECES.Migrations
{
    /// <inheritdoc />
    public partial class updateTablePropertyProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyProducts_Properties_PropertyId",
                table: "PropertyProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyProducts_PropertyValues_PropertyValueId",
                table: "PropertyProducts");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyValueId",
                table: "PropertyProducts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "PropertyProducts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyProducts_Properties_PropertyId",
                table: "PropertyProducts",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyProducts_PropertyValues_PropertyValueId",
                table: "PropertyProducts",
                column: "PropertyValueId",
                principalTable: "PropertyValues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyProducts_Properties_PropertyId",
                table: "PropertyProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyProducts_PropertyValues_PropertyValueId",
                table: "PropertyProducts");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyValueId",
                table: "PropertyProducts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "PropertyProducts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyProducts_Properties_PropertyId",
                table: "PropertyProducts",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyProducts_PropertyValues_PropertyValueId",
                table: "PropertyProducts",
                column: "PropertyValueId",
                principalTable: "PropertyValues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
